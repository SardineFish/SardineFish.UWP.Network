using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.Http
{
    public class HeaderParseException:Exception
    {
        private string headerText;
        public string HeaderText
        {
            get
            {
                return headerText;
            }

            private set
            {
                headerText = value;
            }
        }

        public HeaderParseException(string message):base(message )
        {

        }

        public HeaderParseException(string message,string headerText):base(message )
        {
            HeaderText = headerText;
        }

    }
}
