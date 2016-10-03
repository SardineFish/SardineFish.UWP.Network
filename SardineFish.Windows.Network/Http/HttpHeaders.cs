using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SardineFish.Windows.Network.Http
{
    public class HttpHeaders
    {
        public class RequestLineClass
        {
            public string Methor { get; set; }
            public string URI { get; set; }
            public string HttpVersion { get; set; }

            public RequestLineClass()
            {
                Methor = "";
                HttpVersion = "";
                URI = "";
            }
        }

        public class StatusLineClass
        {
            public int StatusCode { get; set; }
            public string Reason { get; set; }
            public string HttpVersion { get; set; }

            public StatusLineClass()
            {
                StatusCode = 0;
                Reason = "";
                HttpVersion = "";
            }
        }

        public Dictionary<string, string> HeadersFields;

        public StatusLineClass StatusLine { get; set; }
        public RequestLineClass RequestLine { get; set; }

        public string this[string key]
        {
            get
            {
                if (key.ToLower() == "methor")
                {
                    if (RequestLine != null)
                        return RequestLine.Methor.ToString();
                    else
                        return "";
                }
                else if (key.ToLower() == "uri")
                {
                    if (RequestLine != null)
                        return RequestLine.URI;
                    else
                        return "";
                }
                else if (key.ToLower() == "httpversion" || key.ToLower() == "http-version" || key.ToLower() == "version")
                {
                    if (RequestLine != null && StatusLine != null)
                    {
                        if (RequestLine.HttpVersion != "")
                            return RequestLine.HttpVersion;
                        if (StatusLine.HttpVersion != "")
                            return StatusLine.HttpVersion;
                        return "";
                    }
                    else if (RequestLine != null)
                        return RequestLine.HttpVersion;
                    else if (StatusLine != null)
                        return StatusLine.HttpVersion;
                    else
                        return "";
                }
                else if (key.ToLower() == "status")
                {
                    if (StatusLine != null)
                        return StatusLine.StatusCode.ToString();
                    else
                        return "";
                }
                else if (key.ToLower() == "reason")
                {
                    if (StatusLine != null)
                        return StatusLine.Reason;
                    else
                        return "";
                }
                else
                    return this.HeadersFields[key];
            }
            set
            {
                if (key.ToLower() == "methor")
                {
                    if (RequestLine == null)
                        RequestLine = new RequestLineClass();
                    RequestLine.Methor = value;
                }
                else if (key.ToLower() == "uri")
                {
                    if (RequestLine == null)
                        RequestLine = new RequestLineClass();
                    RequestLine.URI = value;
                }
                else if (key.ToLower() == "httpversion" || key.ToLower() == "http-version" || key.ToLower() == "version")
                {
                    if (RequestLine == null)
                        RequestLine = new RequestLineClass();
                    if (StatusLine == null)
                        StatusLine = new StatusLineClass();
                    StatusLine.HttpVersion = value;
                    RequestLine.HttpVersion = value;
                }
                else if (key.ToLower() == "status")
                {
                    if (StatusLine != null)
                        StatusLine = new StatusLineClass();
                    StatusLine.StatusCode = Convert.ToInt32(value);
                }
                else if (key.ToLower() == "reason")
                {
                    if (StatusLine == null)
                        StatusLine = new StatusLineClass();
                    StatusLine.Reason = value;
                }
                else
                    this.HeadersFields[key] = value;
            }
        }

        public string HeadersText { get; set; }

        public HttpHeaders()
        {

        }
        public HttpHeaders(string headersText)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            this.HeadersText = headersText;
            if (headersText == "")
                throw new HeaderParseException("无响应数据", "");
            string[] splitedText = headersText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (splitedText.Length <= 0)
                throw new HeaderParseException("无法解析的Header", headersText);
            string[] firstLineSplitedText = splitedText[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (firstLineSplitedText[0].Contains("HTTP"))
            {
                this.StatusLine = new HttpHeaders.StatusLineClass();
                this.StatusLine.HttpVersion = firstLineSplitedText[0];
                this.StatusLine.StatusCode = Convert.ToInt32(firstLineSplitedText[1]);
                this.StatusLine.Reason = firstLineSplitedText[2];
            }
            else
            {
                this.RequestLine = new HttpHeaders.RequestLineClass();
                this.RequestLine.Methor = firstLineSplitedText[0];
                this.RequestLine.URI = firstLineSplitedText[1];
                this.RequestLine.HttpVersion = firstLineSplitedText[2];
            }
            for (int i = 1; i < splitedText.Length; i++)
            {
                if (splitedText[i] == "")
                    break;
                string[] keyAndValue = splitedText[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                if (keyAndValue.Length < 2)
                    throw new HeaderParseException("服务器回应的Header格式有误", headersText);
                header[keyAndValue[0]] = keyAndValue[1];
            }
            this.HeadersFields = header;
        }
        public HttpHeaders(Dictionary<string, string> header)
        {
            this.HeadersFields = header;
        }

        public override string ToString()
        {
            var type = HttpMessageTypes.Null;
            if (RequestLine != null && StatusLine != null)
            {
                if (RequestLine.HttpVersion != "")
                    type = HttpMessageTypes.Request;
                if (StatusLine.HttpVersion != "")
                    type = HttpMessageTypes.Response;
            }
            else if (RequestLine != null)
                type = HttpMessageTypes.Request;
            else if (StatusLine != null)
                type = HttpMessageTypes.Response;
            var sb = new StringBuilder();
            if(type ==  HttpMessageTypes.Request )
            {
                sb.AppendLine(
                    RequestLine.Methor.ToUpper()
                    + " "
                    + RequestLine.URI
                    + " "
                    + RequestLine.HttpVersion
                    );
            }
            else if(type == HttpMessageTypes.Response)
            {
                sb.Append(
                    StatusLine.HttpVersion
                    + " "
                    + StatusLine.StatusCode
                    + " "
                    + StatusLine.Reason
                    );
            }
            foreach (var item in HeadersFields)
            {
                sb.AppendLine(item.Key + ":" + item.Value);
            }
            return sb.ToString();
        }
    }
}
