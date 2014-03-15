using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain;
using OPCApp.Infrastructure;
using System.Net.Http;

namespace OPCApp.DataService.Common
{
    public  class RestClient
    {
        private static string baseUrl = "http://localhost:1401/Api/";
        private static HttpClient _client;
        private static HttpClient Client
        {
            get
            {
                if (_client==null)
	            {
		             _client=new HttpClient();
                    _client.BaseAddress=new Uri(baseUrl);
	            }
                return _client;
            }
            

        }

        public static PageResult<T> GetPageResult<T>(string url)
        {
            //"Order/Index"
            HttpResponseMessage response = Client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var lst = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                return new PageResult<T>(lst, 100);
            }
            else
            {
                return new PageResult<T>(new List<T>(),0);
            }
        }

       
    }
}
