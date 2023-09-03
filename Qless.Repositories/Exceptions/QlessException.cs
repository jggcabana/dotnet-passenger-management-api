using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Exceptions
{
    public class QlessException : Exception
    {
        public QlessException()
        {

        }

        public QlessException(string message)
            : base(message)
        {

        }

        public QlessException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
