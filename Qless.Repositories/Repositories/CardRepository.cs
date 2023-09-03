using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Qless.Repositories.Data;
using Qless.Repositories.Data.DBModels;
using Qless.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Repositories
{
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {

        public CardRepository(ApplicationDbContext context)
            :base(context)
        {

        }

        public override async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Card.Include(c => c.CardType)
                .AsNoTracking().ToListAsync();
        }

        public override async Task<Card> GetAsync(int Id)
        {
            return await _context.Card.Where(c => c.Id == Id)
                .Include(c => c.CardType).AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<CardType?> GetCardType(int cardTypeId)
        {
            return await _context.CardType.SingleOrDefaultAsync(x => x.Id == cardTypeId);
        }

        public override async Task<Card> UpdateAsync(Card card)
        {
            var existing = await _context.Card.SingleOrDefaultAsync(x => x.Id == card.Id);
            _context.Entry(existing).CurrentValues.SetValues(card);

            //var updateResult = _qlessContext.Card.Update(card);
            await _context.SaveChangesAsync();
            return card;
        }
    }
}
