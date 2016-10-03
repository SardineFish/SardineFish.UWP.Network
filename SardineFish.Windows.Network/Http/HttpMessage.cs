using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SardineFish.Windows.Network.Http
{
    public enum HttpMessageTypes
    {
        Null,
        Request,
        Response
    }

    public enum HttpMethors
    {
        NULL,
        GET,
        HEAD,
        POST,
        PUT,
        DELETE,
        TRACE,
        OPTIONS,
        CONNECT,
        PATCH,
    }


    public class HttpMessage
    {
        public HttpMessageTypes Type
        {
            get;
            set;
        }

        public HttpHeaders Headers { get; set; }

        public byte[] Content { get; set; }

        public string ContentString
        {
            get
            {
                MemoryStream ms = new MemoryStream(Content);
                StreamReader sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
        }

        public string HeadersString
        {
            get
            {
                return Headers.ToString();
            }
        }



        public HttpMessage() : this(HttpMessageTypes.Null, null, new byte[0])
        {

        }

        public HttpMessage(HttpMessageTypes type) : this(type, null, new byte[0])
        {

        }

        public HttpMessage(HttpMessageTypes type, HttpHeaders headers, byte[] content)
        {
            this.Type = type;
            this.Headers = headers;
            this.Content = content;
        }

        public void WriteContent(string text)
        {
            this.Content = new byte[0];
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(text);
            sw.Close();
            ms.Close();
            this.Content = ms.GetBuffer();
        }

        public string ReadContentString()
        {
            return Encoding.UTF8.GetString(this.Content);
        }

        public Stream GetContentStream()
        {
            if (this.Content == null)
                this.Content = new byte[0];
            MemoryStream ms = new MemoryStream(this.Content);
            return ms;
        }

        public new string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Headers.ToString());
            sb.AppendLine("\r\n");
            sb.AppendLine(ReadContentString());
            return sb.ToString();
        }

        public byte[] ToBytes()
        {
            var sb = new StringBuilder();
            sb.Append(Headers.ToString());
            sb.AppendLine("\r\n");
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(sb.ToString());
            sw.Flush();
            ms.Write(Content,0, Content.Length);
            sw.Close();
            return ms.GetBuffer();
            
        }
    }
}
