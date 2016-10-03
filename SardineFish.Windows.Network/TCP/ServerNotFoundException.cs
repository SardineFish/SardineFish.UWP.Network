using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.TCP
{
    class ServerNotFoundException:TCPException
    {
        public ServerNotFoundException (TCPClient client):base(client,"无法连接到目标服务器")
        {

        }
    }
}
