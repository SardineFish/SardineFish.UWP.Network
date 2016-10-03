using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;

namespace SardineFish.Windows.Network.TCP
{
    public class TCPClient
    {

        internal const int ReadBytes = 1024;

        private string hostName;
        public string HostName
        {
            get
            {
                return hostName;
            }

            internal set
            {
                hostName = value;
            }
        }

        private int port;
        public int Port
        {
            get
            {
                return port;
            }

            internal set
            {
                port = value;
            }
        }

        internal bool connected = false;
        public bool Connected
        {
            get
            {
                return connected;
            }

            protected set
            {
                connected = value;
            }
        }

        protected TcpClient tcpClient;
        public TcpClient Socket
        {
            get
            {
                return tcpClient;
            }

            protected set
            {
                tcpClient = value;
            }
        }

        protected SslStream sslStream;
        public SslStream SslStream
        {
            get
            {
                return sslStream;
            }
            set
            {
                sslStream = value;
            }
        }

        protected bool ssl = false;
        public bool Ssl
        {
            get
            {
                return ssl;
            }
            set
            {
                ssl = value;
            }
        }


        public TCPClient()
        {

        }

        public TCPClient (System.Net.Sockets.TcpClient client)
        {
            this.tcpClient = client;
            this.connected = true;
            var ip = client.Client.RemoteEndPoint as System.Net.IPEndPoint;
            this.hostName = ip.Address.ToString();
            this.port = ip.Port;
        }

        public virtual async Task ConnectAsync(string hostName, int port)
        {
            Ssl = false;
            if (sslStream != null)
                sslStream.Dispose();
            sslStream = null;
            HostName = hostName;
            Port = port;
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(hostName, port);
                connected = true;
            }
            catch (Exception ex)
            {
                /*
                var error=SocketError.GetStatus(ex.HResult);
                if (error == SocketErrorStatus.HostNotFound || error == SocketErrorStatus.ConnectionTimedOut)
                    throw new ServerNotFoundException(this);*/
                throw ex;
            }
        }

        public virtual void Connect(string hostName,int port)
        {
            Ssl = false;
            if (sslStream != null)
                sslStream.Dispose();
            sslStream = null;
            HostName = hostName;
            Port = port;
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(hostName, port);
                connected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task ConnectSSLAsync(string hostName, int port)
        {
            Ssl = true;
            HostName = hostName;
            Port = port;
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(hostName, port);
                sslStream = new SslStream(tcpClient.GetStream());
                await sslStream.AuthenticateAsClientAsync(hostName);
                connected = true;
            }
            catch(Exception ex)
            {
                /*var error = SocketError.GetStatus(ex.HResult);
                if (error == SocketErrorStatus.HostNotFound || error == SocketErrorStatus.ConnectionTimedOut)
                    throw new ServerNotFoundException(this);*/
                throw ex;
            }
        }

        public virtual void SendByte(byte data)
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);
                if (!Ssl)
                {
                    using (NetworkStream s = tcpClient.GetStream())
                    {
                        s.WriteByte(data);
                    }
                }
                else
                {
                    SslStream.WriteByte(data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task SendBytesAsync(byte[] data)
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);

                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    await s.WriteAsync(data, 0, data.Length);
                    await s.FlushAsync();
                }
                else
                {
                    await SslStream.WriteAsync(data, 0, data.Length);
                    await SslStream.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void SendBytes(byte[] data)
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);

                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    s.Write(data, 0, data.Length);
                    s.Flush();
                }
                else
                {
                    SslStream.Write(data, 0, data.Length);
                    SslStream.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task SendStringAsync(string data)
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(s);
                    await sw.WriteAsync(data);
                    await sw.FlushAsync();
                }
                else
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(SslStream);
                    await sw.WriteAsync(data);
                    sw.Flush();
                    
                }
            }
            catch (Exception ex)
            {
                /*var error = SocketError.GetStatus(ex.HResult);
                if (error == SocketErrorStatus.ConnectionResetByPeer)
                {
                    Connected = false;
                    throw new DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        public virtual void SendString(string data)
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(s);
                    sw.Write(data);
                    sw.Flush();
                }
                else
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(SslStream);
                    sw.Write(data);
                    sw.Flush();

                }
            }
            catch (Exception ex)
            {
                /*var error = SocketError.GetStatus(ex.HResult);
                if (error == SocketErrorStatus.ConnectionResetByPeer)
                {
                    Connected = false;
                    throw new DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        public virtual async Task<string> RecieveStringAsync()
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    System.IO.StreamReader sr = new System.IO.StreamReader(s);
                    return await sr.ReadToEndAsync();

                    
                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(SslStream);
                    return await sr.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual string RecieveString()
        {
            try
            {
                if (!connected)
                    throw new DisconnectedException(this);
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    System.IO.StreamReader sr = new System.IO.StreamReader(s);
                    return sr.ReadToEnd();


                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(SslStream);
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual byte RecieveByte()
        {
            try
            {
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    int v = s.ReadByte();
                    if (v < 0)
                        throw new System.IO.EndOfStreamException("End");
                    return (byte)v;
                }
                else
                {
                    int v = SslStream.ReadByte();
                    if (v < 0)
                        throw new System.IO.EndOfStreamException("End");
                    return (byte)v;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<byte> RecieveByteAsync()
        {
            try
            {
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    byte[] buffer = new byte[1];
                    int x = await s.ReadAsync(buffer, 0, 1);
                        if (x == 0)
                            throw new System.IO.EndOfStreamException("End");
                    return (byte)buffer[0];
                }
                else
                {
                    byte[] buffer = new byte[1];
                    int x = await SslStream.ReadAsync(buffer, 0, 1);
                    if (x == 0)
                        throw new System.IO.EndOfStreamException("End");
                    return (byte)buffer[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual async Task<int> RecieveBytesAsync(byte[] buffer, int offset,int count)
        {
            try
            {
                if (!Ssl)
                {
                    NetworkStream s = tcpClient.GetStream();
                    return await s.ReadAsync(buffer, offset, count);
                }
                else
                {
                    return await SslStream.ReadAsync(buffer, offset, count);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual int RecieveBytes(byte[] buffer, int offset,int count)
        {
            if (!Ssl)
            {
                NetworkStream s = tcpClient.GetStream();
                return s.Read(buffer, offset, count);
            }
            else
            {
                return sslStream.Read(buffer, offset, count);
            }
        }

    }
}
