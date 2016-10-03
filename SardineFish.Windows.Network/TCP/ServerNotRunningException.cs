using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SardineFish.Windows.Network.TCP
{
    public class ServerNotRunningException: TCPException
    {
        public ServerNotRunningException(TCPServer server) : base(server, "ServerNotRunning.")
        {

        }
    }
}
