using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class NeedReconnectException:HttpRequestExcetpion
    {
        private HttpConnection httpConnection;
        public HttpConnection HttpConnection
        {
            get
            {
                return httpConnection;
            }

            private set
            {
                httpConnection = value;
            }
        }
        public NeedReconnectException(HttpRequest request, HttpConnection connection) : base(request, "请求的服务器与连接的服务器不一致")
        {
            HttpConnection = connection;
        }

        
    }
}
