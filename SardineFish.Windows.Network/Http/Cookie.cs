using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace SardineFish.Windows.Network.Http
{
    [DataContract]
    public class Cookie
    {
        [DataMember(Name = "name", IsRequired = false)]
        string name;
        [DataMember(Name = "value", IsRequired = false)]
        string value;
        [DataMember(Name = "domain", IsRequired = false)]
        string domain;
        [DataMember(Name = "path", IsRequired = false)]
        string path;
        [DataMember(Name = "secure", IsRequired = false)]
        bool secure = false;
        [DataMember(Name = "httpOnly", IsRequired = false)]
        bool httpOnly = false;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public string Domain
        {
            get
            {
                return domain;
            }

            set
            {
                domain = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public bool Secure
        {
            get
            {
                return secure;
            }

            set
            {
                secure = value;
            }
        }

        public bool HttpOnly
        {
            get
            {
                return httpOnly;
            }

            set
            {
                httpOnly = value;
            }
        }

        public Cookie(string name, string value, string domain = "", string path = "/")
        {
            this.Name = name;
            this.Value = value;
            this.Domain = domain;
            this.Path = path;
        }

        public Cookie(string setCookieText,string defaultDomain)
        {
            try
            {
                if(setCookieText =="")
                    throw new CookieParseException(setCookieText);

                var items = setCookieText.Replace(" ", "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Length <= 0)
                    throw new CookieParseException(setCookieText);
                
                var splitedItem = items[0].Split('=');
                if (splitedItem.Length < 2)
                    throw new CookieParseException(setCookieText);

                Name = splitedItem[0];
                Value = splitedItem[1];
                for (long i = 2; i < splitedItem.Length; i++)
                    Value += "=" + splitedItem[i];

                Dictionary<string, string> itemsDic = new Dictionary<string, string>();
                foreach (var item in items)
                {

                    splitedItem = item.Split('=');
                    if (splitedItem.Length < 2)
                    {
                        if (splitedItem[0].ToLower() == "secure")
                            Secure = true;
                        else if (splitedItem[0].ToLower() == "httponly")
                            HttpOnly = true;
                        else
                            throw new CookieParseException(setCookieText);
                    }
                    else
                        itemsDic[splitedItem[0]] = splitedItem[1];

                }
                if (itemsDic.ContainsKey("domain"))
                    Domain = itemsDic["domain"];
                else
                    domain = defaultDomain;

                if (itemsDic.ContainsKey("path"))
                    Path = itemsDic["path"];
                else
                    Path = "/";
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Cookie()
        {

        }
    }
}
