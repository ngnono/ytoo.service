// ***********************************************************************
// Assembly         : OPCApp.DataService
// Author           : Liuyh
// Created          : 03-15-2014 11:09:05
//
// Last Modified By : Liuyh
// Last Modified On : 03-18-2014 12:37:40
// ***********************************************************************
// <copyright file="RestClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Net.Http;
using Intime.OPC.ApiClient;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Common
{
    /// <summary>
    ///     Class RestClient.
    /// </summary>
    public class RestClient
    {
        /// <summary>
        ///     The base URL
        /// </summary>
        private static ApiHttpClient _client;

        /// <summary>
        ///     Gets the client.
        /// </summary>
        /// <value>The client.</value>
        private static ApiHttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    string baseUrl = AppEx.Config.GetValue("apiAddress");
                    string consumerKey = AppEx.Config.GetValue("consumerKey");
                    string consumerSecret = AppEx.Config.GetValue("consumerSecret");

                    var factory = new DefaultApiHttpClientFactory(new Uri(baseUrl), consumerKey, consumerSecret);
                    _client = factory.Create();
                }
                return _client;
            }
        }

        /// <summary>
        ///     SetToken
        /// </summary>
        /// <param name="token"></param>
        public static void SetToken(string token)
        {
            Client.SetToken(token);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="urlParams"></param>
        /// <returns></returns>
        public static IList<T> Get<T>(string address, string urlParams = "")
        {
            string url = string.Format("{0}?{1}", address, urlParams);
            HttpResponseMessage response = Client.GetAsync(url).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<List<T>>().Result : new List<T>();
        }

        public static PageResult<T> GetPage<T>(string address, string urlParams)
        {
            string url = string.Format("{0}?{1}", address, urlParams);
            HttpResponseMessage response = Client.GetAsync(url).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<PageResult<T>>().Result : new PageResult<T>(null, 10);
        }
        public static PageResult<T> Get<T>(string address, string urlParams, int pageIndex, int pageSize)
        {
            string url = string.Format("{0}?{1}&pageIndex={2}&pageSize={3}", address, urlParams, pageIndex, pageSize);
            HttpResponseMessage response = Client.GetAsync(url).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<PageResult<T>>().Result: new PageResult<T>(null,10);
        }
        public static T GetSingle<T>(string address, string urlParams = "")
        {
            string url = string.IsNullOrWhiteSpace(urlParams) ? address : string.Format("{0}?{1}", address, urlParams);
            HttpResponseMessage response = Client.GetAsync(url).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default(T);
        }

        /// <summary>
        ///     Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Post<T>(string url, T data)
        {
            HttpResponseMessage response = Client.PostAsJsonAsync(url, data).Result;
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns>``1.</returns>
        public static TResult Post<T, TResult>(string url, T data)
        {
            HttpResponseMessage response = Client.PostAsJsonAsync(url, data).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<TResult>().Result : default(TResult);
        }


        /// <summary>
        ///     Puts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Put<T>(string url, T data)
        {
            HttpResponseMessage response = Client.PutAsJsonAsync(url, data).Result;
            return response.IsSuccessStatusCode;
        }
        public static T PutReturnModel<T>(string url, T data)
        {
            HttpResponseMessage response = Client.PutAsJsonAsync(url, data).Result;
            return response.IsSuccessStatusCode ? response.Content.ReadAsAsync<T>().Result : default(T); ;
        }
        public static bool Put(string url, object data)
        {
            HttpResponseMessage response = Client.PutAsJsonAsync(url, data).Result;
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Deletes the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Delete<T>(string url)
        {
            HttpResponseMessage response = Client.DeleteAsync(url).Result;
            return response.IsSuccessStatusCode;
        }
    }
}