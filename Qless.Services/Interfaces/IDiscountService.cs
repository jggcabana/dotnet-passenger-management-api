using Qless.Repositories.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Services.Interfaces
{
    public interface IDiscountService
    {
        public Task<IEnumerable<Discount>> GetEligibleDiscounts(Card card);

        public Task ComputeCharges(Card card);
    }
}
