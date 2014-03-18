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
using System.Configuration;
using Intime.OPC.ApiClient;
using System.Net.Http;
using OPCApp.Infrastructure;

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
        private static ApiHttpClient _client;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        private static ApiHttpClient Client
        {
            get
            {

                if (_client == null)
                {
                    string  baseUrl = AppEx.Config.GetValue("apiAddress");
                    string consumerKey = AppEx.Config.GetValue("consumerKey");
                    string consumerSecret = AppEx.Config.GetValue("consumerSecret");

                    var factory=new DefaultApiHttpClientFactory(new Uri(baseUrl), consumerKey, consumerSecret);
                    _client = factory.Create();

                }
                return _client;
            }
        }

        /// <summary>
        /// Gets the page result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
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

            var response = Client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return  response.Content.ReadAsAsync<List<T>>().Result;
            }
            else
            {
                return new List<T>();
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
            var response = Client.PostAsJsonAsync(url, data).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
          
        }

        /// <summary>
        /// Posts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="data">The data.</param>
        /// <returns>``1.</returns>
        public static TResult Post<T, TResult>(string url, T data)
        {
            var response = Client.PostAsJsonAsync(url, data).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<TResult>().Result;
            }
            else
            {
                return default(TResult);
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

            var response = Client.PutAsJsonAsync(url, data).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
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

            var response = Client.DeleteAsync(url).Result;
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
