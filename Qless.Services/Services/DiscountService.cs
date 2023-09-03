using Qless.Repositories.Data.DBModels;
using Qless.Repositories.Interfaces;
using Qless.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Services.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepo)
        {
            _discountRepository = discountRepo ?? throw new ArgumentNullException(nameof(discountRepo));
        }

        public async Task<IEnumerable<Discount>> GetEligibleDiscounts(Card card)
        {
            var discountList = await _discountRepository.GetAllAsync(card.CardTypeId);

            var validDiscounts = new List<Discount>();

            foreach (Discount discount in discountList)
            {
                if (discount.StartDate.HasValue && discount.StartDate.Value > DateTime.UtcNow)
                    continue;

                if (discount.EndDate.HasValue && discount.EndDate.Value < DateTime.UtcNow)
                    continue;

                if (discount.AvailmentLimit != -1 && card.TripCount >= discount.AvailmentLimit)
                    continue;

                if (discount.Type == "special" && !IsSpecialDiscountValid(card, discount))
                    continue;


                validDiscounts.Add(discount);
            }

            return validDiscounts;
        }

        public async Task ComputeCharges(Card card)
        {
            decimal percentageDiscount = 0;
            decimal flatDiscount = 0;

            var eligibleDiscounts = new List<Discount>();

            if (card.CardTypeId == 3)
                eligibleDiscounts = (await GetEligibleDiscounts(card)).ToList();
            foreach (var discount in eligibleDiscounts)
            {
                if (discount.IsFlatDiscount)
                    flatDiscount += discount.DiscountValue;
                else
                    percentageDiscount += discount.DiscountValue;
            }
            card.CurrentBalance -= card.CardType.BaseRate * (1 - (percentageDiscount / 100)) - flatDiscount;
        }

        private bool IsSpecialDiscountValid(Card card, Discount discount)
        {
            bool isValid = false;
            switch (discount.Name)
            {
                case "succeeding":
                    if (card.LastUsed?.Date == DateTime.Today.Date && card.TripCountToday >= 1 && card.TripCountToday <= discount.AvailmentLimitPerDay)
                        isValid = true;
                    break;
                default:
                    isValid = false;
                    break;
            }

            return isValid;
        }
    }
}