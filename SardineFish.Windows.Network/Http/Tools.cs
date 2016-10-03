using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace SardineFish.Windows.Network.Http
{
    public static class Tools
    {
        public static string UrlEncode(string text)
        {
            return System.Web.HttpUtility.UrlEncode(text);
        }

        public static string UrlDecode(string text)
        {
            return System.Web.HttpUtility.UrlDecode(text);
        }

        public static string JsonEncode(object obj)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                ds.WriteObject(ms, obj);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static T JsonDecode<T>(string jsonText)
        {
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonText));
            T obj = (T)ds.ReadObject(ms);
            return obj;
        }
    }
}
