using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.ComponentModel.Composition;
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
        private const string TokenKey = "X-Token";

        private readonly Uri _baseAddress;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly Random _random = new Random();
        private readonly MediaTypeFormatter _mediaTypeFormatter = new JsonMediaTypeFormatter();

        [Import]
        public IEventAggregator EventAggregator { get; set;}

        public RestClient(string baseAddress, string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _baseAddress = new Uri(baseAddress);
            _consumerSecret = consumerSecret;
        }

        public string Token { get; set; }

        public TData Get<TData>(string uri)
        {
            using (var client = CreateHttpClient())
            {
                SetHttpHeader(client);

                return Request<TData>(uri, client.GetAsync);
            }
        }

        public TEntity Post<TEntity>(string uri, TEntity entity)
        {
            using (var client = CreateHttpClient())
            {
                SetHttpHeader(client, entity);

                return Send<TEntity>(uri, entity, client.PostAsJsonAsync);
            }
        }

        public TEntity Put<TEntity>(string uri, TEntity entity)
        {
            using (var client = CreateHttpClient())
            {
                SetHttpHeader(client, entity);

                return Send<TEntity>(uri, entity, client.PutAsJsonAsync);
            }
        }

        public void Delete(string uri)
        {
            using (var client = CreateHttpClient())
            {
                SetHttpHeader(client);

                Request<string>(uri, client.DeleteAsync);
            }
        }

        private TData Request<TData>(string uri, Func<string, Task<HttpResponseMessage>> verb)
        {
            var randomString = BuildRandomString<TData>();
            var url = string.Format("{0}{1}{2}", _baseAddress, uri, randomString);
            var response = verb(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<TData>().Result;
            }

            var errorMessage = response.Content.ReadAsStringAsync().Result;
            throw new RestException(response.StatusCode, errorMessage);
        }

        private TEntity Send<TEntity>(string uri, TEntity entity, Func<string, TEntity, Task<HttpResponseMessage>> verb)
        {
            var response = verb(string.Format("{0}{1}", _baseAddress, uri), entity).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<TEntity>().Result;
            }

            var errorMessage = response.Content.ReadAsStringAsync().Result;
            throw new RestException(response.StatusCode, errorMessage);
        }

        private void SetHttpHeader(HttpClient client, object entity = null)
        {
            SetHeaderValue(client, Consumerkey, _consumerKey);
            SetHeaderValue(client, TokenKey, Token);
            if (entity != null)
            { 
                SetHeaderValue(client, Signature, Sign(entity)); 
            }
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

        private static string BuildRandomString<TData>()
        {
            var randomString = string.Empty;
            var dataType = typeof(TData);
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(PagedResult<>))
            {
                randomString = string.Format("&timestamp={0}", DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            }
            else
            {
                randomString = string.Format("/?timestamp={0}", DateTime.Now.ToString("MMddyyyyHHmmssfff"));
            }
            return randomString;
        }

        private string Sign<TEntity>(TEntity entity)
        {
            var builder = new StringBuilder();
            if (entity != null)
            {
                var data = GetDataString(entity);
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
