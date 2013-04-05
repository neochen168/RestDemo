using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RestDemo
{
    class Program
    {
        public static void TestDynRestApi()
        {
            String url = "https://api2.dynect.net/REST/Session/";
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

            req.Method = "POST";
            req.ContentType = "application/json";

            string json = "{\"customer_name\":\"modedge\"," + "\"user_name\":\"bgriffin\"," +
              "\"password\":\"modedge2013\"}";

            StringBuilder paramz = new StringBuilder();
            paramz.Append("customer_name=");
            paramz.Append(HttpUtility.UrlEncode("modedge"));
            paramz.Append("&");
            paramz.Append("user_name=");

            paramz.Append(HttpUtility.UrlEncode("bgriffin"));
            paramz.Append("&");

            paramz.Append("password=");
            paramz.Append(HttpUtility.UrlEncode("modedge2013"));

            byte[] formData = UTF8Encoding.UTF8.GetBytes(paramz.ToString());
            formData = UTF8Encoding.UTF8.GetBytes(json);

            req.ContentLength = formData.Length;

            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }

            String result = null;
            using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            result = result.Replace('"', ' ');
            String[] results = result.Split(':');
            bool isFound = false;
            string token = string.Empty;
            foreach (string str in results)
            {
                if (isFound)
                {
                    isFound = false;
                    token = str.Split(',')[0].Trim();
                    break;
                }

                if (str.Trim().Contains("token"))
                {
                    isFound = true;
                }
            }

            Console.WriteLine(token);

            String getZoneUrl = "https://api2.dynect.net/REST/Zone/modedgetest1.com/";

            req = WebRequest.Create(getZoneUrl) as HttpWebRequest;
            req.Method = "GET";
            req.ContentType = "application/json";


            json = ",{\"Auth-Token\":\"" + "token" + "\"}";
            req.Headers.Add("Auth-Token", token);

            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
                Console.WriteLine(result);
            }


            getZoneUrl = "https://api2.dynect.net/REST/AllRecord/modedgetest1.com/";

            req = WebRequest.Create(getZoneUrl) as HttpWebRequest;
            req.Method = "GET";
            req.ContentType = "application/json";


            json = ",{\"Auth-Token\":\"" + "token" + "\"}";
            req.Headers.Add("Auth-Token", token);

            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
                Console.WriteLine(result);
            }
        }


        static void Main(string[] args)
        {
            //TestDynRestApi();

            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();
            wrapper.GetZone();

            //wrapper.AddZone("modedgetest.com");
            //wrapper.PublishZone("modedgetest.com");
            //wrapper.AddZone();
            //wrapper.GetZone("neotest.com");
            //wrapper.PublishZone("neotest.com");
            //wrapper.PublishZone();
            //wrapper.DeleteZones();

            //wrapper.AddRecord("AAAA", "neotest.com", "sharepoint.neotest.com", 3600, "2607:f8b0:400e:c02::68");
            //wrapper.AddRecord("NS", "neotest.com", "nodata2.neotest.com", 3600, "ns1.p07.dynnet.com");
           // wrapper.AddRecord("CNAME", "neotest.com", "sharepoint2.neotest.com", 3600, "www.outlook.com");
            //wrapper.AddRecord("A", "neotest.com", "lync.neotest.com", 3600, "127.0.0.1");
            //wrapper.PublishZone("neotest.com");
        }
    }
}
