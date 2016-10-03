using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SardineFish.Windows.Network.Http
{
    public class CookieContainer
    {
        private Dictionary<string, Dictionary<string, Cookie>> domains = new Dictionary<string, Dictionary<string, Cookie>>();
        public Dictionary<string, Dictionary<string, Cookie>> Domains
        {
            get
            {
                return domains;
            }

            set
            {
                domains = value;
            }
        }

        public Dictionary<string, Cookie> this[string index]
        {
            get
            {
                return Domains[index];
            }
            set
            {
                Domains[index] = value;
            }
        }
        
        public CookieContainer()
        {
            domains = new Dictionary<string, Dictionary<string, Cookie>>();
        }

        public CookieContainer(List<Cookie> cookieList)
        {
            foreach (var cookie in cookieList)
            {
                AddCookie(cookie);
            }

        }

        public void AddCookie(Cookie cookie)
        {
            if (Domains == null)
                Domains = new Dictionary<string, Dictionary<string, Cookie>>();
            if (!Domains.ContainsKey(cookie.Domain))
                Domains.Add(cookie.Domain, new Dictionary<string, Cookie>());
            Domains[cookie.Domain][cookie.Name] = cookie;
        }

        public List<Cookie> GetCookies(string requestUrl)
        {
            try
            {
                List<Cookie> list = new List<Cookie>();
                foreach(var domain in Domains )
                {
                    Regex reg = new Regex(domain.Key.Replace(".", "\\."));
                    if (reg.IsMatch(requestUrl))
                    {
                        foreach (var cookie in domain.Value)
                        {
                            list.Add(cookie.Value);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Cookie> GetCookies()
        {
            try
            {
                List<Cookie> list = new List<Cookie>();
                foreach (var domain in Domains)
                {
                    foreach (var cookie in domain.Value)
                    {
                        list.Add(cookie.Value);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
