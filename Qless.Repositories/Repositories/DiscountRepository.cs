using Microsoft.EntityFrameworkCore;
using Qless.Repositories.Data;
using Qless.Repositories.Data.DBModels;
using Qless.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Repositories
{
    public class DiscountRepository : BaseRepository<Discount>, IDiscountRepository
    {
        public DiscountRepository(ApplicationDbContext context)
            :base(context)
        {

        }

        public async Task<IEnumerable<Discount>> GetAllAsync(int cardTypeId)
        {
            return await _context.Discount
                .Include(d => d.CardTypes.Where(ct => ct.Id == cardTypeId))
                .ToListAsync();
        }
    }
}
