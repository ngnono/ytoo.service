using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.REST
{
    public class RestClient : IRestClient
    {
        private readonly Uri _baseAddress;
        private readonly string _privateKey;
        private readonly string _from;
        private readonly Random _random = new Random();
        private readonly MediaTypeFormatter _mediaTypeFormatter = new JsonMediaTypeFormatter();

        public RestClient(string baseAddress, string privateKey, string from)
        {
            _privateKey = privateKey;
            _baseAddress = new Uri(baseAddress);
            _from = from;
        }

        public TResponse Get<TResponse>(string uri)
        {
            using (var client = CreateHttpClient())
            {
                return Request<TResponse>(uri, client.GetAsync);
            }
        }

        public TResponse Post<TData, TResponse>(Request<TData> request)
        {
            using (var client = CreateHttpClient())
            {
                return Send<TData, TResponse>(request, client.PostAsJsonAsync);
            }
        }

        public TResponse Put<TData, TResponse>(Request<TData> request)
        {
            using (var client = CreateHttpClient())
            {
                return Send<TData, TResponse>(request, client.PutAsJsonAsync);
            }
        }

        public TResponse Delete<TResponse>(string uri)
        {
            using (var client = CreateHttpClient())
            {
                return Request<TResponse>(uri, client.DeleteAsync);
            }
        }

        private TResponse Request<TResponse>(string uri, Func<string, Task<HttpResponseMessage>> verb)
        {
            var response = verb(string.Format("{0}{1}&ram={2}", _baseAddress, uri, _random.Next(0, 99))).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<TResponse>().Result;
            }

            return default(TResponse);
        }

        private TResponse Send<TData, TResponse>(Request<TData> request, Func<string, Request<TData>, Task<HttpResponseMessage>> verb)
        {
            request.From = _from;
            request.Nonce = _random.Next(0, 99);
            request.Timestamp = DateTime.Now.ToString("s");
            request.Signature = Sign(request);

            var response = verb(string.Format("{0}{1}", _baseAddress, request.URI), request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<TResponse>().Result;
            }

            return default(TResponse);
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient { BaseAddress = _baseAddress };
        }

        /// <summary>
        /// 计算签名
        /// </summary>
        /// <typeparam name="TRequest">请求对象</typeparam>
        /// <typeparam name="TResponse">响应对象</typeparam>
        /// <param name="request">请求实体</param>
        /// <returns>具体的签名</returns>
        private string Sign<TRequest>(Request<TRequest> request)
        {
            // 参数列表
            var list = new List<string>(3)
            {
                request.From,
                request.Nonce.ToString(CultureInfo.InvariantCulture),
                request.Timestamp
            };

            // 参数排序
            list.Sort();

            var builder = new StringBuilder();
            builder.Append(string.Join("", list));

            // 进行数据的串行化
            if (request.Data != null)
            {
                var data = GetDataString(request.Data);
                builder.Append(data);
            }

            return ComputeHash(_privateKey, builder.ToString());
        }

        private string GetDataString<T>(T data)
        {
            using (var content = new ObjectContent(typeof(T), data, _mediaTypeFormatter))
            {
                return content.ReadAsStringAsync().Result;
            }
        }

        private string ComputeHash(string privateKey, string message)
        {
            var key = Encoding.UTF8.GetBytes(privateKey);

            using (var hmac = new HMACSHA1(key))
            {
                hmac.Initialize();
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
