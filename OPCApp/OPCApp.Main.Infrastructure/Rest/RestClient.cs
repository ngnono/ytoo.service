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
        private const string Signature = "X-Sign";
        private const string Consumerkey = "X-ConsumerKey";
        private const string Token = "X-Token";

        private readonly Uri _baseAddress;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Random _random = new Random();
        private readonly MediaTypeFormatter _mediaTypeFormatter = new JsonMediaTypeFormatter();

        public RestClient(string baseAddress, string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _baseAddress = new Uri(baseAddress);
            _consumerSecret = consumerSecret;
        }

        public TResponse Get<TResponse>(string uri)
        {
            using (var client = CreateHttpClient())
            {
                SetHeaderValue(client, Consumerkey, _consumerKey);

                return Request<TResponse>(uri, client.GetAsync);
            }
        }

        public TResponse Post<TData, TResponse>(Request<TData> request)
        {
            using (var client = CreateHttpClient())
            {
                SetHeaderValue(client, Consumerkey, _consumerKey);
                SetHeaderValue(client, Signature, Sign(request));

                return Send<TData, TResponse>(request, client.PostAsJsonAsync);
            }
        }

        public TResponse Put<TData, TResponse>(Request<TData> request)
        {
            using (var client = CreateHttpClient())
            {
                SetHeaderValue(client, Consumerkey, _consumerKey);
                SetHeaderValue(client, Signature, Sign(request));

                return Send<TData, TResponse>(request, client.PutAsJsonAsync);
            }
        }

        public TResponse Delete<TResponse>(string uri)
        {
            using (var client = CreateHttpClient())
            {
                SetHeaderValue(client, Consumerkey, _consumerKey);

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

        private void SetHeaderValue(HttpClient client, string name, string value)
        {
            lock (client.DefaultRequestHeaders)
            {
                client.DefaultRequestHeaders.Remove(name);
                client.DefaultRequestHeaders.Add(name, value);
            }
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
            var builder = new StringBuilder();
            // 进行数据的串行化
            if (request.Data != null)
            {
                var data = GetDataString(request.Data);
                builder.Append(data);
            }
            builder.Append(_consumerKey);
            builder.Append(_consumerSecret);

            return ComputeHash(builder.ToString());
        }

        private string GetDataString<T>(T data)
        {
            using (var content = new ObjectContent(typeof(T), data, _mediaTypeFormatter))
            {
                return content.ReadAsStringAsync().Result;
            }
        }

        private string ComputeHash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            var sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
