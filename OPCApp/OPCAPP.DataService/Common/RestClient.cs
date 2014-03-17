// ***********************************************************************
// Assembly         : OPCApp.DataService
// Author           : Liuyh
// Created          : 03-15-2014 11:09:05
//
// Last Modified By : Liuyh
// Last Modified On : 03-16-2014 00:29:15
// ***********************************************************************
// <copyright file="RestClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using Intime.OPC.ApiClient;
using System.Net.Http;

namespace OPCApp.DataService.Common
{
    /// <summary>
    /// Class RestClient.
    /// </summary>
    public  class RestClient
    {
        /// <summary>
        /// The base URL
        /// </summary>
        private static string baseUrl = ConfigurationManager.AppSettings["apiAddress"];
        /// <summary>
        /// The _client factory
        /// </summary>
        private static DefaultApiHttpClientFactory _clientFactory;
        /// <summary>
        /// Gets the client factory.
        /// </summary>
        /// <value>The client factory.</value>
        private static DefaultApiHttpClientFactory ClientFactory
        {
            get
            {
                if (_clientFactory == null)
	            {
                    _clientFactory = new DefaultApiHttpClientFactory(new Uri(baseUrl), "100000001", "test001");
                    
	            }
                return _clientFactory;
            }
        }

        /// <summary>
        /// Gets the page result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>PageResult{``0}.</returns>
        //public static PageResult<T> GetPageResult<T>(string url)
        //{
        //"Order/Index"
        //HttpResponseMessage response = Client.GetAsync(url).Result;
        //if (response.IsSuccessStatusCode)
        //{
        //    var lst = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
        //    return new PageResult<T>(lst, 100);
        //}
        //else
        //{
        //    return new PageResult<T>(new List<T>(),0);
        //}
        //}

        public static IList<T> Get<T>(string url, object obj)
        {
            using (var client = ClientFactory.Create())
            {

                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return  response.Content.ReadAsAsync<List<T>>().Result;
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Post<T>(string url,T data)
        {
            using (var client = ClientFactory.Create())
            {

                var response = client.PostAsJsonAsync(url, data).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static TResult Post<T, TResult>(string url, T data)
        {
            using (var client = ClientFactory.Create())
            {

                var response = client.PostAsJsonAsync(url, data).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<TResult>().Result;
                }
                else
                {
                    return default(TResult);
                }
            }
        }

       

        /// <summary>
        /// Puts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Put<T>(string url, T data)
        {
            using (var client = ClientFactory.Create())
            {

                var response = client.PutAsJsonAsync(url, data).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Delete<T>(string url)
        {
            using (var client = ClientFactory.Create())
            {

                var response = client.DeleteAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}
