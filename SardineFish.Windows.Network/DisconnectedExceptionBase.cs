using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network
{
    public class DisconnectedExceptionBase:Exception
    {
        private object errorObject;
        public object ErrorObject
        {
            get
            {
                return errorObject;
            }

            private set
            {
                errorObject = value;
            }
        }

        public DisconnectedExceptionBase(object errorObject = null) : base()
        {
            ErrorObject = errorObject;
        }

        public DisconnectedExceptionBase(string message, object errorObject = null):this(message,null,errorObject)
        {
        }

        public DisconnectedExceptionBase(string message, Exception innerException, object errorObject = null) : base(message, innerException)
        {
            this.ErrorObject = errorObject;
        }
    }
}
