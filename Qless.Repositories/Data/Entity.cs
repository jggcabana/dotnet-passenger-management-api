using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Data
{
    public abstract class Entity 
    {
        public int Id { get; init; }

        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; } = null;
    }
}
