using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Data.DBModels
{
    public class Discount : Entity
    {
        //public int DiscountId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsFlatDiscount { get; set; }
        public int AvailmentLimit { get; set; }
        public int AvailmentLimitPerDay { get; set; }

        public virtual ICollection<CardType> CardTypes { get; set; }
    }
}
