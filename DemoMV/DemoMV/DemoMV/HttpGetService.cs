using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DemoMV
{
    class HttpGetService
    {
        private WebClient webClient;
        private string _url;

        public HttpGetService(string url)
        {
            webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            this._url = url;
        }

        public string URL
        {
            get { return _url; }
            set { this._url = value; }
        }

        public void AddPrameter(string id, string value)
        {
            webClient.QueryString.Add(id, value);
        }

        public string Request() 
        {
            string response;
            try
            {

                Stream data = webClient.OpenRead(_url);
                StreamReader reader = new StreamReader(data);
                response = reader.ReadToEnd();
                data.Close();
                reader.Close();
                if (webClient.QueryString.Count != 0)
                {
                    webClient.QueryString.Clear();
                }
                return response;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}
