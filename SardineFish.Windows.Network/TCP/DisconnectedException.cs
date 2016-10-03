using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SardineFish.Windows.Network.TCP
{
    public class DisconnectedException:DisconnectedExceptionBase
    {
        private TCPClient errorObject;

        public new TCPClient ErrorObject
        {
            get
            {
                return errorObject;
            }

            set
            {
                errorObject = value;
            }
        }

        public DisconnectedException(TCPClient tcpClient = null) : base("未建立连接或连接已断开")
        {
            this.errorObject = tcpClient;
        }

        public DisconnectedException(string message, TCPClient tcpClient = null) : base(message)
        {
            this.errorObject = tcpClient;
        }

        public DisconnectedException(string message, Exception innerException, TCPClient tcpClient = null) : base(message, innerException)
        {
            this.errorObject = tcpClient;
        }
    }
}
