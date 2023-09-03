using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Data.DBModels
{
    public class Card : Entity
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public Guid CardId { get; set; }  

        public int CardTypeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ExpiresByDate { get; set; }

        public DateTime? LastUsed { get; set; }

        public decimal CurrentBalance { get; set; }

        public bool InTransit { get; set; }

        public int TripCount { get; set; }

        public int TripCountToday { get; set; }

        public virtual CardType CardType { get; set; }
    }
}
