using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class HttpRequestExcetpion:Exception
    {
        private HttpRequest request;
        public HttpRequest Request
        {
            get
            {
                return request;
            }

            private set
            {
                request = value;
            }
        }

        public HttpRequestExcetpion(HttpRequest request) : base()
        {
            Request = request;
        }

        public HttpRequestExcetpion(HttpRequest request, string message) : base(message)
        {
            Request = request;
        }

        
    }
}
