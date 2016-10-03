using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class RequestNotSentException:HttpRequestExcetpion
    {
        public RequestNotSentException(HttpRequest request) : base(request, "Http请求尚未发出")
        {

        }
    }
}
