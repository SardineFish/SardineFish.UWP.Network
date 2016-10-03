using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.TCP
{
    public class TCPException : Exception
    {
        private TCPClient client;
        public TCPClient Client
        {
            get
            {
                return client;
            }

            protected set
            {
                client = value;
            }
        }

        public TCPServer Server
        {
            get;
            protected set;
        }



        public TCPException(TCPClient client) : base()
        {
            Client = client;
        }

        public TCPException(TCPClient client, string messsage) : base(messsage)
        {
            Client = client;
        }

        public TCPException(TCPServer server) : base()
        {
            Server = server;
        }

        public TCPException (TCPServer server,string message) : base(message)
        {
            Server = server;
        }
    }
}
