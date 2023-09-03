using Qless.Repositories.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Interfaces
{
    public interface ICardRepository
    {
        public Task<IEnumerable<Card>> GetAllAsync();

        public Task<Card> GetAsync(int cardId);

        public Task<Card> AddAsync(Card card);

        public Task<CardType?> GetCardType(int cardTypeId);

        public Task<Card> UpdateAsync(Card card);
    }
}
