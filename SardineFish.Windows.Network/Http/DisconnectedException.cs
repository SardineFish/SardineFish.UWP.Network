using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    public class DisconnectedException:DisconnectedExceptionBase
    {
        private HttpRequest errorObject;
        public DisconnectedException(HttpRequest request = null) : base("连接未建立或已断开")
        {
            this.errorObject = request;
        }

        public DisconnectedException(string message, HttpRequest request = null) : base(message)
        {
            this.errorObject = request;
        }

        public DisconnectedException(string message, Exception innerException, HttpRequest request = null) : base(message, innerException)
        {
            this.errorObject = request;
        }

        public new HttpRequest ErrorObject
        {
            get
            {
                return errorObject;
            }

            set
            {
                errorObject = value;
            }
        }
    }
}
