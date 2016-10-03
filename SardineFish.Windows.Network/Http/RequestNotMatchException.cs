using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class RequestNotMatchException:Exception
    {
        private HttpRequest lastRequest;
        public HttpRequest LastRequest
        {
            get
            {
                return lastRequest;
            }

            private set
            {
                lastRequest = value;
            }
        }

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


        public RequestNotMatchException(HttpRequest lastRequest, HttpRequest request) : base("该Http请求与原Http请求不是同一实例")
        {
            LastRequest = lastRequest;
            Request = request;
        }

    }
}
