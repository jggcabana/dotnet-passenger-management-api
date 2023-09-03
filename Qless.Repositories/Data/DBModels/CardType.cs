using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Data.DBModels
{
    public class CardType : Entity
    {
        //public int CardTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal BaseRate { get; set; }

        public decimal StartingBalance { get; set; }

        public decimal MaximumBalance { get; set; }

        public int MaximumIdleDurationYears { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }
    }
}
