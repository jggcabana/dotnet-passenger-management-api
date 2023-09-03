using Qless.Repositories.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        public Task<IEnumerable<Discount>> GetAllAsync();
        public Task<IEnumerable<Discount>> GetAllAsync(int cardTypeId);
        public Task<Discount> GetAsync(int discountId);
    }
}
