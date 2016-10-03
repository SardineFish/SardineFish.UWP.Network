using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;
using SardineFish.Windows.Network.TCP;

namespace SardineFish.Windows.Network.Http
{
    public class HttpRequest
    {
        #region const
        internal const string HeaderKeyContentType = "Content-Type";
        internal const string HeaderKeyAccept = "Accept";
        internal const string HeaderKeyHost = "Host";
        internal const string HeaderKeyContentLength = "Content-Length";
        internal const string HeaderKeyConnection = "Connection";
        internal const string HeaderKeyReferer = "Referer";
        internal const string HeaderKeyUserAgent = "User-Agent";

        internal const string MethodGet = "GET";
        internal const string MethodPost = "POST";
        #endregion

        #region Properties

        internal HttpConnection httpConnection;
        public HttpConnection HttpConnection
        {
            get
            {
                return httpConnection;
            }

            set
            {
                httpConnection = value;
            }
        }

        internal string url;
        public string Url
        {
            get
            {
                return url;
            }
            private set
            {
                url = value;
                var uri = new Uri(url);
                this.Host = uri.Host;
                if (uri.Scheme == "https")
                    Secure = true;
                this.RelativeUrl = value.Replace("https://", "").Replace("http://", "").Replace(uri.Host, "");
                this.Port = uri.Port;

            }
        }

        internal string method;
        public string Method
        {
            get
            {
                return method.ToUpper();
            }

            set
            {
                method = value.ToUpper();
            }
        }

        public bool Connected
        {
            get
            {
                return HttpConnection.Connected;
            }
        }

        private Dictionary<string, string> headers;
        public Dictionary<string, string> Headers
        {
            get
            {
                return headers;
            }

            set
            {
                headers = value;
            }
        }


        private CookieContainer cookieContainer = new CookieContainer();
        public CookieContainer CookieContainer
        {
            get
            {
                return cookieContainer;
            }

            set
            {
                cookieContainer = value;
            }
        }


        private string relativeUrl;
        public string RelativeUrl
        {
            get
            {
                return relativeUrl;
            }

            private set
            {
                relativeUrl = value;
            }
        }



        public string ContentType
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyContentType))
                    return "";
                return Headers[HeaderKeyContentType];
            }
            set
            {
                Headers[HeaderKeyContentType] = value;
            }
        }

        public string Accept
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyAccept))
                    return "";
                return Headers[HeaderKeyAccept];
            }
            set
            {
                Headers[HeaderKeyAccept] = value;
            }
        }

        public string Host
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyHost))
                    return "";
                return Headers[HeaderKeyHost];
            }
            set
            {
                Headers[HeaderKeyHost] = value;
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

        public long ContentLength
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyContentLength))
                    return 0;
                return Convert.ToInt64(Headers[HeaderKeyContentLength]);
            }
            internal set
            {
                Headers[HeaderKeyContentLength] = value.ToString();
            }
        }

        public string Connection
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyConnection))
                    return "";
                return Headers[HeaderKeyConnection];
            }
            set
            {
                Headers[HeaderKeyConnection] = value;
            }
        }

        public string Referer
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyReferer))
                    return "";
                return Headers[HeaderKeyReferer];
            }
            set
            {
                Headers[HeaderKeyReferer] = value;
            }
        }
        public string UserAgent
        {
            get
            {
                if (!headers.ContainsKey(HeaderKeyUserAgent))
                    return "";
                return Headers[HeaderKeyUserAgent];
            }
            set
            {
                Headers[HeaderKeyUserAgent] = value;
            }
        }
        

        public string PostText
        {
            get
            {
                return Encoding.UTF8.GetString(this.Content);
            }

            set
            {
                this.Content = Encoding.UTF8.GetBytes(value);
            }
        }

        private byte[] content = new byte[0];
        public byte[] Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }


        private bool sent = false;
        public bool Sent
        {
            get
            {
                return sent;
            }

            internal set
            {
                sent = value;
            }
        }

        private bool allowRedirect = true;
        public bool AllowRedirect
        {
            get
            {
                return allowRedirect;
            }

            set
            {
                allowRedirect = value;
            }
        }

        private bool secure = false;
        public bool Secure
        {
            get
            {
                return secure;
            }

            set
            {
                secure = value;
                if (value == true)
                    Port = 443;
            }
        }



        #endregion

        public HttpRequest(string url)
        {
            this.headers = new Dictionary<string, string>();
            this.CookieContainer = new CookieContainer();
            try
            {
                this.Url = url;
                HttpConnection = new HttpConnection(Host, Port, Secure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HttpRequest(HttpConnection httpConnection, string url, string methor, bool reconnect, bool connectNow = false)
        {
            this.httpConnection = httpConnection;

            this.headers = new Dictionary<string, string>();
            this.CookieContainer = new CookieContainer();
            try
            {
                this.Url = url;
                this.Method = methor;
                if (Host != httpConnection.HostName || Secure != httpConnection.Secure || Port != httpConnection.Port)
                {
                    if (reconnect)
                    {
                        HttpConnection = new HttpConnection(Host, Port);
                        if (connectNow)
                            ConnectAsync().Wait();
                    }
                    else
                        throw new NeedReconnectException(this, httpConnection);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HttpRequest(string url, string methor, bool connectNow = false)
        {
            this.headers = new Dictionary<string, string>();
            this.CookieContainer = new CookieContainer();
            try
            {
                this.Url = url;
                this.Method = methor;
                HttpConnection = new HttpConnection(Host, Port, Secure);
                if (connectNow)
                {
                    ConnectAsync().Wait();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Methors


        public async Task<HttpConnection> ConnectAsync()
        {
            try
            {
                if (Secure)
                    await HttpConnection.ConnectSSLAsync(Host, Port);
                else
                    await HttpConnection.ConnectAsync(Host, Port);
                return HttpConnection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendAsync()
        {
            await SendAsync("");
        }

        public void Send()
        {
            SendAsync().Wait();
        }

        public async Task SendAsync(string postText)
        {
            try
            {
                this.PostText = postText;
                if (this.HttpConnection == null)
                {
                    this.HttpConnection = new HttpConnection(Host, Port);
                    await HttpConnection.ConnectAsync();
                }
                if (!HttpConnection.Connected)
                {
                    await HttpConnection.ConnectAsync(Host, Port);
                    if (!HttpConnection.Connected)
                        throw new DisconnectedException("未建立连接或连接已中断", this);
                }
                await HttpConnection.SendRequestAsync(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Send(string postText)
        {
            SendAsync(postText).Wait();
        }

        public async Task SendAsync(byte[] data)
        {
            try
            {
                this.Content = data;
                if (this.HttpConnection == null)
                {
                    this.HttpConnection = new HttpConnection(Host, Port);
                    await HttpConnection.ConnectAsync();
                }
                if (!HttpConnection.Connected)
                {
                    await HttpConnection.ConnectAsync(Host, Port);
                    if (!HttpConnection.Connected)
                        throw new DisconnectedException("未建立连接或连接已中断", this);
                }
                await HttpConnection.SendRequestAsync(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Send(byte[] data)
        {
            SendAsync(data).Wait();
        }

        public async Task<HttpResponse > GetResponseAsync()
        {
            try
            {
                if (HttpConnection == null || !Sent)
                {
                    throw new RequestNotSentException(this);
                }
                if (!HttpConnection.Connected)
                    throw new DisconnectedException("未建立连接或连接已中断", this);
                return await HttpConnection.GetResponseAsync(this);
            }
            catch
            {
                throw;
            }
        }

        public HttpResponse GetResponse()
        {
            try
            {
                if (HttpConnection == null || !Sent)
                {
                    throw new RequestNotSentException(this);
                }
                if (!HttpConnection.Connected)
                    throw new DisconnectedException("未建立连接或连接已中断", this);
                return HttpConnection.GetResponse(this);
            }
            catch
            {
                throw;
            }
        }

        public async Task<HttpResponse> SendRquestAndGetResponseAsync(string postText = "")
        {
            await SendAsync(postText);
            return await GetResponseAsync();
        }

        public HttpResponse SendRquestAndGetResponse(string postText = "")
        {
            Send(postText);
            return GetResponse();
        }

        #endregion

    }
}
