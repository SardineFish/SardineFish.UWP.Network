using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class RedirectException : HttpRequestExcetpion
    {
        private HttpResponse response;
        public HttpResponse Response
        {
            get
            {
                return response;
            }

            private set
            {
                response = value;
            }
        }
        public string RedirectTo
        {
            get
            {
                if (response == null)
                {
                    return "";
                }
                return response.Location;
            }
        }


        public RedirectException(HttpRequest request,HttpResponse response):base(request ,"页面已被重定向且未允许对其进行处理")
        {
            Response = response;
        }
    }
}
