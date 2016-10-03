using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Web;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SardineFish.Windows.Network.TCP
{
    public class TCPServer
    {
        public TcpListener Listener { get; set; }

        public bool Running
        {
            get;
            protected set;
        }

        public delegate Task ListenCallbackAsync(SardineFish.Windows.Network.TCP.TCPClient client);

        public delegate void ListenCallback(SardineFish.Windows.Network.TCP.TCPClient client);

        public TCPServer()
        {
            this.Running = false;
        }

        public virtual SardineFish.Windows.Network.TCP.TCPClient AcceptTCPClient()
        {
            try
            {
                return new SardineFish.Windows.Network.TCP.TCPClient(Listener.AcceptTcpClient());
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.Interrupted)
                    this.Running = false;
                throw ex;
            }
        }

        public virtual void Setup(int port)
        {
            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();
            this.Running = true;
        }

        public virtual async void ListenAsync(int port, ListenCallbackAsync callback)
        {
            try
            {
                Listener = new TcpListener(IPAddress.Any, port);
                Listener.Start();
                this.Running = true;
                while (Running)
                {
                    var client = await Listener.AcceptTcpClientAsync();
                    if (!this.Running)
                        return;
                    await callback(new SardineFish.Windows.Network.TCP.TCPClient(client));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Listen(int port,ListenCallback callback)
        {
            try
            {
                Listener = new TcpListener(IPAddress.Any, port);
                Listener.Start();
                this.Running = true;
                while (true)
                {
                    var client = Listener.AcceptTcpClient();
                    if (!this.Running)
                        return;
                    callback(new SardineFish.Windows.Network.TCP.TCPClient(client));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Shutdown()
        {
            this.Listener.Stop();
            this.Running = false;
        }

        
    }

    
}
