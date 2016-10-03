using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    class CookieParseException:Exception
    {
        private string setCookieText;
        public string SetCookieText
        {
            get
            {
                return setCookieText;
            }

            private set
            {
                setCookieText = value;
            }
        }

        public CookieParseException(string setCookieText) : base("Set-Cookie文本格式有误")
        {
            SetCookieText = setCookieText;
        }
    }
}
