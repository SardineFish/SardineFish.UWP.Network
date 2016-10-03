using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SardineFish.Windows.Network.Http
{
    public class HttpConnection:TCP.TCPClient
    {
        
        private bool secure = false;
        public bool Secure
        {
            get
            {
                return secure;
            }

            set
            {
                if (secure == false && value == true)
                {
                    Port = 443;
                    connected = false;
                }
                else if (secure == true && value == false)
                {
                    Port = 80;
                    connected = false;
                }
                secure = value;
            }
        }

        private bool allowRetry = true;
        public bool AllowRetry
        {
            get
            {
                return allowRetry;
            }

            set
            {
                allowRetry = value;
            }
        }

        private int retryLimit = 5;
        public int RetryLimit
        {
            get
            {
                return retryLimit;
            }

            set
            {
                retryLimit = value;
            }
        }

        private int timeOut = 10000;
        public int TimeOut
        {
            get
            {
                return timeOut;
            }

            set
            {
                timeOut = value;
            }
        }

        private int retryCount = 0;

        private HttpRequest lastRequest;
        public HttpRequest LastRequest
        {
            get
            {
                return lastRequest;
            }

            private set
            {
                lastRequest = value;
            }
        }

        

        public HttpConnection(string hostName, int port = 80, bool secure = false) : base()
        {
            HostName = hostName;
            Port = port;
            Secure = secure;
        }

        public HttpConnection(TcpClient client)
        {
            this.Socket = client;
            this.connected = client.Connected;
            if(client.Connected)
            {
                var ip = client.Client.RemoteEndPoint as System.Net.IPEndPoint;
                this.HostName = ip.Address.ToString();
                this.Port = ip.Port;
            }
        }

        public virtual async Task ConnectAsync()
        {
            await ConnectAsync(HostName, Port);
        }

        public virtual void Connect()
        {
            Connect(HostName, Port);
        }

        public override Task ConnectAsync(string hostName, int port)
        {
            Retry:
            try
            {
                if (Secure == true)
                    return base.ConnectSSLAsync(hostName, port);
                return base.ConnectAsync(hostName, port);
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        public virtual async Task ConnectSSLAsync()
        {
            await ConnectSSLAsync(HostName, Port);
        }
        
        public override Task ConnectSSLAsync(string hostName, int port)
        {
            Retry:
            try
            {

                if (Secure == false)
                    return base.ConnectAsync(hostName, port);
                return base.ConnectSSLAsync(hostName, port);
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        protected virtual async Task<string> RecieveHeaderAsync()
        {
            string headerText = "";
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                char r;
                while (true)
                {
                    r = (char)await RecieveByteAsync();
                    headerText += r;
                    if (headerText.Contains("\r\n\r\n") || headerText.Contains("\r\r") || headerText.Contains("\n\n"))
                        break;
                }
                return headerText;
            }
            catch(Exception ex)
            {
                /*var error = Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult);
                if (error == Windows.Networking.Sockets.SocketErrorStatus.ConnectionResetByPeer || (uint)ex.HResult == 0x80000013)
                {
                    Connected = false;
                    throw new TCP.DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        protected virtual string RecieveHeader()
        {
            string headerText = "";
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                char r;
                while (true)
                {
                    r = (char)RecieveByte();
                    headerText += r;
                    if (headerText.Contains("\r\n\r\n") || headerText.Contains("\r\r") || headerText.Contains("\n\n"))
                        break;
                }
                return headerText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual async Task<byte[]> RecieveContentAsync(long length)
        {
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                byte[] buffer = new byte[length];
                int x = 0;
                int l = (int)(length % ((long)int.MaxValue + 1));
                length -= l;
                while (l > 0)
                {
                    x = 0;
                    while (x < l)
                    {
                        x += await RecieveBytesAsync(buffer, x, l);
                    }
                    l = (int)(length % ((long)int.MaxValue + 1));
                    length -= l;
                }
                return buffer;
            }
            catch(Exception ex)
            {
                /*var error = Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult);
                if (error == Windows.Networking.Sockets.SocketErrorStatus.ConnectionResetByPeer || (uint)ex.HResult == 0x80000013)
                {
                    Connected = false;
                    throw new TCP.DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        protected virtual byte[] RecieveContent(long length)
        {
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                var lengthTotal = length;
                byte[] buffer = new byte[length];
                int x = 0;
                int l = (int)(length % ((long)int.MaxValue + 1));
                length -= l;
                while (l > 0)
                {
                    x = 0;
                    while (x < l)
                    {

                        x += RecieveBytes(buffer, x, (int)(l - x));
                    }
                    l = (int)(length % ((long)int.MaxValue + 1));
                    length -= l;
                }
                return buffer;
            }
            catch (Exception ex)
            {
                /*var error = Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult);
                if (error == Windows.Networking.Sockets.SocketErrorStatus.ConnectionResetByPeer || (uint)ex.HResult == 0x80000013)
                {
                    Connected = false;
                    throw new TCP.DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }



        protected virtual async Task<byte[]> RecieveContentChunkAsync()
        {
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                List<byte[]> bufferList = new List<byte[]>();
                string lengthText = "";
                char r;
                long length = 0;
                long l = long.MaxValue;
                while (l > 0)
                {
                    while (true)
                    {
                        r = (char)await RecieveByteAsync();
                        lengthText += r;
                        if (lengthText.Contains("\r\n"))
                            break;
                    }
                    lengthText = lengthText.Replace("\r", "").Replace("\n", "");
                    l = Convert.ToInt64(lengthText, 16);
                    length += l;
                    if (l <= 0)
                        break;
                    bufferList.Add(await RecieveContentAsync(l));
                    lengthText = "";
                    while (true)
                    {
                        r = (char)await RecieveByteAsync();
                        if (r != '\r' && r != '\n')
                        {
                            lengthText += r;
                            break;
                        }
                    }
                }
                byte[] buffer = new byte[length];
                int index = 0;
                foreach (byte[] b in bufferList)
                {
                    b.CopyTo(buffer, index);
                    index += b.Length;
                }

                return buffer;
            }
            catch(Exception ex)
            {
                /*var error = Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult);
                if (error == Windows.Networking.Sockets.SocketErrorStatus.ConnectionResetByPeer || (uint)ex.HResult == 0x80000013)
                {
                    Connected = false;
                    throw new TCP.DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        protected virtual byte[] RecieveContentChunk()
        {
            try
            {
                if (!Connected || Socket == null)
                    throw new TCP.DisconnectedException(this);
                List<byte[]> bufferList = new List<byte[]>();
                string lengthText = "";
                char r;
                long length = 0;
                long l = long.MaxValue;
                while (l > 0)
                {
                    while (true)
                    {
                        r = (char) RecieveByte();
                        lengthText += r;
                        if (lengthText.Contains("\r\n"))
                            break;
                    }
                    lengthText = lengthText.Replace("\r", "").Replace("\n", "");
                    l = Convert.ToInt64(lengthText, 16);
                    length += l;
                    if (l <= 0)
                        break;
                    bufferList.Add(RecieveContent(l));
                    lengthText = "";
                    while (true)
                    {
                        r = (char)RecieveByte();
                        if (r != '\r' && r != '\n')
                        {
                            lengthText += r;
                            break;
                        }
                    }
                }
                byte[] buffer = new byte[length];
                int index = 0;
                foreach (byte[] b in bufferList)
                {
                    b.CopyTo(buffer, index);
                    index += b.Length;
                }

                return buffer;
            }
            catch (Exception ex)
            {
                /*var error = Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult);
                if (error == Windows.Networking.Sockets.SocketErrorStatus.ConnectionResetByPeer || (uint)ex.HResult == 0x80000013)
                {
                    Connected = false;
                    throw new TCP.DisconnectedException("连接被中断", this);
                }*/
                throw ex;
            }
        }

        public virtual async Task<HttpResponse> SendRequestAndGetResponseAsync(HttpRequest request)
        {
            await SendRequestAsync(request);
            return await GetResponseAsync(request);
        }

        public virtual HttpResponse SendRequestAndGetResponse(HttpRequest request)
        {
            SendRequest(request);
            return GetResponse(request);
        }

        public virtual async Task SendRequestAsync(HttpRequest request)
        {
            Retry:
            try
            {
                if (!Connected)
                {
                    if (!AllowRetry)
                        throw new TCP.DisconnectedException(this);
                    if (request.Secure)
                    {
                        await this.ConnectSSLAsync(request.Host, request.Port);
                    }
                    else
                        await this.ConnectAsync(request.Host, request.Port);
                    if (!Connected)
                        throw new TCP.DisconnectedException(this);
                }
                StringBuilder sb = new StringBuilder();
                string firstline = request.Method + " " + request.RelativeUrl+" HTTP/1.1";
                /*if (Secure)
                    firstline += " HTTPS/1.1";
                else
                    firstline += " HTTP/1.1";*/
                sb.AppendLine(firstline);
                if (request.Accept == "")
                    request.Accept = "*/*";
                if (request.Connection == "")
                    request.Connection = "Keep-Alive";
                if (request.UserAgent == "")
                    request.UserAgent = "Mozilla/5.0 (Windows NT 5.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"; //"Mozilla /5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520)";
                if (request.Method == HttpRequest.MethodPost)
                {
                    request.ContentLength = Encoding.UTF8.GetBytes(request.PostText).Length;
                }
                else
                {
                    request.ContentLength = 0;
                }
                string cookieText = "";
                var cookieList = request.CookieContainer.GetCookies(request.Url);
                foreach (var cookie in cookieList)
                {
                    cookieText += cookie.Name + "=" + cookie.Value + "; ";
                }
                request.Headers["Cookie"] = cookieText;
                foreach (var item in request.Headers)
                {
                    sb.AppendLine(item.Key + ":" + item.Value);
                }
                sb.Append("\r\n");
                if (request.Method == HttpRequest.MethodPost)
                {
                    //sb.Append(request.PostText);
                }
                string text = sb.ToString();
                await this.SendStringAsync(sb.ToString());
                await this.SendBytesAsync(request.Content);
                LastRequest = request;
                request.Sent = true;
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    Connected = false;
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        public virtual void SendRequest(HttpRequest request)
        {
            SendRequestAsync(request).Wait();
        }

        public virtual async Task<HttpResponse> GetResponseHeaderAsync(HttpRequest request)
        {
            Retry:
            if (lastRequest == null)
            {
                if (!AllowRetry)
                    throw new RequestNotSentException(request);
                await this.SendRequestAsync(request);
            }
            if (request != lastRequest)
            {
                if (!AllowRetry)
                    throw new RequestNotMatchException(LastRequest, request);
                await this.SendRequestAsync(request);
            }
            if (!Connected)
            {
                if (!AllowRetry)
                    throw new TCP.DisconnectedException(this);
                await this.SendRequestAsync(request);
            }
            try
            {
                HttpResponse response = new HttpResponse(request);
                string headerText = await RecieveHeaderAsync();
                response.headerText = headerText;
                if (headerText == "")
                    throw new HeaderParseException("无响应数据", "");
                string[] splitedText = headerText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (splitedText.Length <= 0)
                    throw new HeaderParseException("无法解析的Header", headerText);
                string[] firstLineSplitedText = splitedText[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (firstLineSplitedText.Length < 3)
                    throw new HeaderParseException("服务器回应的Header格式有误", headerText);
                response.Method = firstLineSplitedText[0];
                try
                {
                    response.Status = Convert.ToInt32(firstLineSplitedText[1]);
                }
                catch
                {
                    throw new HeaderParseException("无法解析Status的值", headerText);
                }
                response.Reason = firstLineSplitedText[2];
                if (response.header == null)
                    response.header = new Dictionary<string, string>();
                for (int i = 1; i < splitedText.Length; i++)
                {
                    if (splitedText[i] == "")
                        break;
                    string[] keyAndValue = splitedText[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyAndValue.Length < 2)
                        throw new HeaderParseException("服务器回应的Header格式有误", headerText);
                    if (keyAndValue[0] == HttpResponse.HeaderSetCookie)
                    {
                        Cookie cookie = new Cookie(keyAndValue[1], request.Host);
                        if (response.SetCookie == null)
                            response.SetCookie = new CookieContainer();
                        response.SetCookie.AddCookie(cookie);
                        request.CookieContainer.AddCookie(cookie);
                        continue;
                    }
                    response.Header[keyAndValue[0]] = keyAndValue[1];
                }
                response.Url = request.Url;
                return response;
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    Connected = false;
                    goto Retry;
                }
                else
                    throw ex;
            }
        }
        public virtual HttpResponse GetResponseHeader(HttpRequest request)
        {
            Retry:
            if (lastRequest == null)
            {
                if (!AllowRetry)
                    throw new RequestNotSentException(request);
                this.SendRequest(request);
            }
            if (request != lastRequest)
            {
                if (!AllowRetry)
                    throw new RequestNotMatchException(LastRequest, request);
                this.SendRequest(request);
            }
            if (!Connected)
            {
                if (!AllowRetry)
                    throw new TCP.DisconnectedException(this);
                this.SendRequest(request);
            }
            try
            {
                HttpResponse response = new HttpResponse(request);
                string headerText = RecieveHeader();
                response.headerText = headerText;
                if (headerText == "")
                    throw new HeaderParseException("无响应数据", "");
                string[] splitedText = headerText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (splitedText.Length <= 0)
                    throw new HeaderParseException("无法解析的Header", headerText);
                string[] firstLineSplitedText = splitedText[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (firstLineSplitedText.Length < 3)
                    throw new HeaderParseException("服务器回应的Header格式有误", headerText);
                response.Method = firstLineSplitedText[0];
                try
                {
                    response.Status = Convert.ToInt32(firstLineSplitedText[1]);
                }
                catch
                {
                    throw new HeaderParseException("无法解析Status的值", headerText);
                }
                response.Reason = firstLineSplitedText[2];
                if (response.header == null)
                    response.header = new Dictionary<string, string>();
                for (int i = 1; i < splitedText.Length; i++)
                {
                    if (splitedText[i] == "")
                        break;
                    string[] keyAndValue = splitedText[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyAndValue.Length < 2)
                        throw new HeaderParseException("服务器回应的Header格式有误", headerText);
                    if (keyAndValue[0] == HttpResponse.HeaderSetCookie)
                    {
                        Cookie cookie = new Cookie(keyAndValue[1], request.Host);
                        if (response.SetCookie == null)
                            response.SetCookie = new CookieContainer();
                        response.SetCookie.AddCookie(cookie);
                        request.CookieContainer.AddCookie(cookie);
                        continue;
                    }
                    response.Header[keyAndValue[0]] = keyAndValue[1];
                }
                response.Url = request.Url;
                return response;
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    Connected = false;
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        public virtual async Task<HttpResponse> GetResponseAsync(HttpRequest request)
        {
            Retry:
            if (lastRequest == null)
            {
                if (!AllowRetry)
                    throw new RequestNotSentException(request);
                await this.SendRequestAsync(request);
            }
            if (request != lastRequest)
            {
                if (!AllowRetry)
                    throw new RequestNotMatchException(LastRequest, request);
                await this.SendRequestAsync(request);
            }
            if (!Connected)
            {
                if (!AllowRetry)
                    throw new TCP.DisconnectedException(this);
                await this.SendRequestAsync(request);
            }
            try
            {
                var response = await GetResponseHeaderAsync(request);
                //--------------------------------------------------------------Redirect
                if (response.Status == 302 || response.Status == 301)
                {
                    if (!request.AllowRedirect)
                    {
                        throw new RedirectException(request, response);
                    }
                    else
                    {
                        if(!response.Location .Contains("http://")&&!response.Location.Contains ("https://"))
                        {
                            Uri u = new Uri(response.Url);
                            response.Location = u.Scheme + "://" + u.Host + "/" + response.Location;
                        }
                        HttpRequest requestRd = new HttpRequest(response.Location, "GET", false);

                        requestRd.CookieContainer = request.CookieContainer;
                        var responseRd = await requestRd.SendRquestAndGetResponseAsync();
                        responseRd.redirectHistory.Add(response.Location);
                        return responseRd;
                    }
                }
                if (response.ContentLength < 0)
                    throw new HeaderParseException("错误的Content-Length");

                //--------------------------------------------------------------Get Content
                if (response.TransferEncoding==HttpResponse.TransferEncodingChunked)
                {
                    response.ContentBuffer = await RecieveContentChunkAsync();
                }
                else
                    response.ContentBuffer = await RecieveContentAsync(response.ContentLength);
                lastRequest = null;
                return response;
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    Connected = false;
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        public virtual HttpResponse GetResponse(HttpRequest request)
        {
            Retry:
            if (lastRequest == null)
            {
                if (!AllowRetry)
                    throw new RequestNotSentException(request);
                this.SendRequest(request);
            }
            if (request != lastRequest)
            {
                if (!AllowRetry)
                    throw new RequestNotMatchException(LastRequest, request);
                this.SendRequest(request);
            }
            if (!Connected)
            {
                if (!AllowRetry)
                    throw new TCP.DisconnectedException(this);
                this.SendRequest(request);
            }
            try
            {
                var response = GetResponseHeader(request);
                //--------------------------------------------------------------Redirect
                if (response.Status == 302 || response.Status == 301)
                {
                    if (!request.AllowRedirect)
                    {
                        throw new RedirectException(request, response);
                    }
                    else
                    {
                        if (!response.Location.Contains("http://") && !response.Location.Contains("https://"))
                        {
                            Uri u = new Uri(response.Url);
                            response.Location = u.Scheme + "://" + u.Host + "/" + response.Location;
                        }
                        HttpRequest requestRd = new HttpRequest(response.Location, "GET", false);

                        requestRd.CookieContainer = request.CookieContainer;
                        var responseRd = requestRd.SendRquestAndGetResponse();
                        responseRd.redirectHistory.Add(response.Location);
                        return responseRd;
                    }
                }
                if (response.ContentLength < 0)
                    throw new HeaderParseException("错误的Content-Length");

                //--------------------------------------------------------------Get Content
                if (response.TransferEncoding == HttpResponse.TransferEncodingChunked)
                {
                    response.ContentBuffer = RecieveContentChunk();
                }
                else
                    response.ContentBuffer = RecieveContent(response.ContentLength);
                lastRequest = null;
                return response;
            }
            catch (Exception ex)
            {
                if (AllowRetry)
                {
                    retryCount++;
                    if (RetryLimit > 0 && retryCount > RetryLimit)
                    {
                        throw ex;
                    }
                    Connected = false;
                    goto Retry;
                }
                else
                    throw ex;
            }
        }

        public virtual HttpHeaders RecieveHttpHeaders()
        {
            return new HttpHeaders(RecieveHeader());
        }

        public virtual HttpMessage RecieveHttpMessage()
        {
            if (!Connected)
                throw new TCP.DisconnectedException(this);
            HttpMessage httpmessage = new HttpMessage();
            httpmessage.Headers = RecieveHttpHeaders();
            if (httpmessage.Headers.RequestLine != null)
                httpmessage.Type = HttpMessageTypes.Request;
            else if (httpmessage.Headers.StatusLine != null)
                httpmessage.Type = HttpMessageTypes.Response;
            //chunked
            if (httpmessage.Headers.HeadersFields.ContainsKey("Transfer-Encoding") && httpmessage.Headers.HeadersFields["Transfer-Encoding"].ToLower() == "chunked")
            {
                httpmessage.Content = RecieveContentChunk();
                httpmessage.Headers.HeadersFields.Remove("Transfer-Encoding");
                httpmessage.Headers.HeadersFields["Content-Length"] = httpmessage.Content.Length.ToString();
            }
            else if (httpmessage.Headers.HeadersFields.ContainsKey("Content-Length"))
            {
                httpmessage.Content = RecieveContent(Convert.ToInt64(httpmessage.Headers.HeadersFields["Content-Length"]));
            }
            return httpmessage;
        }

        public virtual void SendHttpMessage(HttpMessage message)
        {
            if (!Connected)
                throw new TCP.DisconnectedException(this);
            if(message.Headers.HeadersFields.ContainsKey("Transfer-Encoding") && message.Headers.HeadersFields["Transfer-Encoding"].ToLower() == "chunked")
            {
                message.Headers.HeadersFields.Remove("Transfer-Encoding");
                message.Headers.HeadersFields["Content-Length"] = message.Content.Length.ToString();
            }
            var headersText = message.Headers.ToString() + "\r\n";
            var contentText = message.ReadContentString();

            var text = headersText + contentText;

            this.SendString(headersText);
            this.SendBytes(message.Content);
        }
    }
}
