using Qless.Repositories.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Services.Interfaces
{
    public interface ICardService
    {
        public Task<IEnumerable<Card>> GetCards();

        public Task<Card> GetCard(int cardId);

        //public Task<Card> CreateCard(CreateCardRequest request);

        public Task<Card> CreateCard(int cardTypeId, string idNo);

        public Task<Card> LoadCard(int cardId, decimal newBalance);
            
        public Task<Card> ExitStation(Card card);

        public Task<Card> EnterStation(Card card);
    }
}
