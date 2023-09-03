using Qless.Repositories.Data.DBModels;
using Qless.Repositories.Enums;
using Qless.Repositories.Exceptions;
using Qless.Repositories.Interfaces;
using Qless.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Services.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IDiscountService _discountService;

        public CardService(ICardRepository cardRepo, IDiscountService discountService)
        {
            _cardRepository = cardRepo ?? throw new ArgumentNullException(nameof(cardRepo));
            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
        }

        public async Task<Card> CreateCard(int cardTypeId, string idNo)
        {
            // TODO: save senior/pwd number (sensitive data??)
            if (cardTypeId == (int)CardTypeEnum.Discounted && string.IsNullOrWhiteSpace(idNo))
                throw new QlessException("Must provide PWD or Senior Citizen ID # to avail discounted card.");

            var currentBalance = (await _cardRepository.GetCardType(cardTypeId)).StartingBalance;
            var newCard = new Card
            {
                CardTypeId = cardTypeId,
                CreatedDate = DateTime.UtcNow,
                CurrentBalance = currentBalance
            };

            return await _cardRepository.AddAsync(newCard);
        }

        public async Task<Card> GetCard(int cardId)
        {
            return await _cardRepository.GetAsync(cardId);
        }

        public async Task<IEnumerable<Card>> GetCards()
        {
            return await _cardRepository.GetAllAsync();
        }

        public async Task<Card> ExitStation(Card card)
        {
            var cardType = await _cardRepository.GetCardType(card.CardTypeId);

            if (!card.InTransit)
                throw new QlessException("Card not in transit.");

            var idleTimeYears = !card.LastUsed.HasValue ? 0 : (DateTime.UtcNow - card.LastUsed.Value).TotalDays / 365;
            if (idleTimeYears > cardType.MaximumIdleDurationYears)
                throw new QlessException("Card is expired.");

            //var baseRate = cardType.BaseRate;
            //card.CurrentBalance -= baseRate;

            await _discountService.ComputeCharges(card);

            if (DateTime.UtcNow.Date > card.LastUsed?.Date)
                card.TripCountToday = 1;
            else
                ++card.TripCountToday;

            card.LastUsed = DateTime.UtcNow;
            card.InTransit = !card.InTransit;
            ++card.TripCount;

            return await _cardRepository.UpdateAsync(card);
        }

        public async Task<Card> EnterStation(Card card)
        {
            var cardType = await _cardRepository.GetCardType(card.CardTypeId);

            if (card.InTransit)
                throw new QlessException("Unable to enter: Card is in transit.");

            var idleTimeYears = !card.LastUsed.HasValue ? 0 : (DateTime.UtcNow - card.LastUsed.Value).TotalDays / 365;
            if (idleTimeYears > cardType.MaximumIdleDurationYears)
                throw new QlessException("Unable to enter: Card is expired.");

            if (card.CurrentBalance - cardType.BaseRate < 0)
                throw new QlessException("Unable to enter: Insufficient Balance.");

            card.LastUsed = DateTime.UtcNow;
            card.InTransit = !card.InTransit;

            return await _cardRepository.UpdateAsync(card);
        }

        public async Task<Card> LoadCard(int cardId, decimal newBalance)
        {
            var card = await _cardRepository.GetAsync(cardId);
            if (card == null)
                throw new QlessException("Invalid card.");

            if (newBalance > card.CardType.MaximumBalance)
                throw new QlessException($"Card balance limit must not exceed {card.CardType.MaximumBalance}");

            if (card.CurrentBalance > newBalance)
                throw new QlessException("New balance cant be less than current balance.");

            card.CurrentBalance = newBalance;
            return await _cardRepository.UpdateAsync(card);
        }
    }
}
