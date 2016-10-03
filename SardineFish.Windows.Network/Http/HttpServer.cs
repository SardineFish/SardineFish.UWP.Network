using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SardineFish.Windows.Network.TCP;

namespace SardineFish.Windows.Network.Http
{
    public class HttpServer : TCP.TCPServer
    {
        public new delegate void ListenCallback(HttpConnection connection);
        public void Listen(int port, ListenCallback callback)
        {
            try
            {

                base.Setup(port);
                while (true)
                {
                    var client = base.AcceptTCPClient();
                    callback(new HttpConnection(client.Socket));
                }
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                if (socketEx.SocketErrorCode == System.Net.Sockets.SocketError.Interrupted)
                    return;
            }
        }
    }
}
