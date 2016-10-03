using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Sockets;

namespace SardineFish.Windows.Network.Http
{
    public class HttpResponse
    {
        #region const

        internal const string HeaderKeyContentType = "Content-Type";
        internal const string HeaderKeyContentLength = "Content-Length";
        internal const string HeaderKeyDate = "Date";
        internal const string HeaderKeyConnection = "Connection";
        internal const string HeaderKeyLocation = "Location";
        internal const string HeaderTransferEncoding = "Transfer-Encoding";
        internal const string HeaderSetCookie = "Set-Cookie";

        internal const string TransferEncodingChunked = "chunked";
        #endregion

        internal string headerText;
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

        internal HttpRequest request;
        public HttpRequest Request
        {
            get
            {
                return request;
            }

            internal set
            {
                request = value;
            }
        }

        internal int status;
        public int Status
        {
            get
            {
                return status;
            }

            internal set
            {
                status = value;
            }
        }

        internal string reason;
        public string Reason
        {
            get
            {
                return reason;
            }

            internal set
            {
                reason = value;
            }
        }

        internal string method;
        public string Method
        {
            get
            {
                return method.ToUpper();
            }

            internal set
            {
                method = value.ToUpper();
            }
        }

        private string url;
        public string Url
        {
            get
            {
                return url;
            }

            internal set
            {
                url = value;
            }
        }

        private CookieContainer setCookie;
        public CookieContainer SetCookie
        {
            get
            {
                return setCookie;
            }

            internal set
            {
                setCookie = value;
            }
        }

        public string ContentType
        {
            get
            {
                if (!header.ContainsKey(HeaderKeyContentType))
                    return "";
                return header[HeaderKeyContentType];
            }
        }

        public long ContentLength
        {
            get
            {
                if (!header.ContainsKey(HeaderKeyContentLength))
                    return 0;
                return Convert.ToInt64(header[HeaderKeyContentLength]);
            }
        }

        public string Connection
        {
            get
            {
                if (!header.ContainsKey(HeaderKeyConnection))
                    return "";
                return header[HeaderKeyConnection];
            }
        }

        public string Location
        {
            get
            {
                if (header == null || !header.ContainsKey(HeaderKeyLocation))
                    return "";
                return header[HeaderKeyLocation];
                
            }
            internal set
            {
                header[HeaderKeyLocation] = value;
            }
        }

        public string TransferEncoding
        {
            get
            {
                if (header == null || !header.ContainsKey(HeaderTransferEncoding))
                    return "";
                return header[HeaderTransferEncoding];
            }
        }

        public string Date
        {
            get
            {
                if (!header.ContainsKey(HeaderKeyDate))
                    return "";
                return header[HeaderKeyDate];
            }
        }

        private byte[] contentBuffer = new byte[0];
        public byte[] ContentBuffer
        {
            get
            {
                return contentBuffer;
            }

            internal set
            {
                contentBuffer = value;
            }
        }

        internal Dictionary<string, string> header = new Dictionary<string, string>();
        public Dictionary<string, string> Header
        {
            get
            {
                return header;
            }

            internal set
            {
                header = value;
            }
        }

        internal List<string> redirectHistory = new List<string>();
        public List<string> RedirectHistory
        {
            get
            {
                return redirectHistory;
            }

            internal set
            {
                redirectHistory = value;
            }
        }

        public HttpConnection HttpConnection
        {
            get
            {
                return request.httpConnection;
            }
        }

        

        public HttpResponse(HttpRequest request)
        {
            this.header = new Dictionary<string, string>();
            this.SetCookie = new CookieContainer();
            this.redirectHistory = new List<string>();
            Request = request;
        }

        public string GetContentText()
        {
            return Encoding.UTF8.GetString(contentBuffer);
        }
    }
}
