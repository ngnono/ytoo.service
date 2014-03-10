// ***********************************************************************
// Assembly         : OPCAPP.DataService
// Author           : Liuyh
// Created          : 03-10-2014 21:30:49
//
// Last Modified By : Liuyh
// Last Modified On : 03-10-2014 21:35:03
// ***********************************************************************
// <copyright file="RestClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OPCAPP.DataService.Common
{
    /// <summary>
    /// Class RestClient.
    /// </summary>
     class RestClient
    {
        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The object.</param>
        /// <returns>``0.</returns>
         public static T Get<T>(string url, object obj = null)
         {
             return default(T);
         }

         /// <summary>
         /// Posts the specified URL.
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="url">The URL.</param>
         /// <param name="obj">The object.</param>
         /// <returns>``0.</returns>
         public static T Post<T>(string url, object obj = null)
         {
             return default(T);
         }
         /// <summary>
         /// Puts the specified URL.
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="url">The URL.</param>
         /// <param name="obj">The object.</param>
         /// <returns>``0.</returns>
         public static T Put<T>(string url, object obj = null)
         {
             return default(T);
         }
         /// <summary>
         /// Deletes the specified URL.
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="url">The URL.</param>
         /// <param name="obj">The object.</param>
         /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
         public static bool Delete<T>(string url, object obj = null)
         {
             return true;
         }
    }

     /// <summary>
     /// Class RestResult.
     /// </summary>
   public  class RestResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RestResult"/> is succes.
        /// </summary>
        /// <value><c>true</c> 如果 succes; 否则, <c>false</c>.</value>
       public bool  Succes { get; set; }
       /// <summary>
       /// Gets or sets the MSG.
       /// </summary>
       /// <value>The MSG.</value>
       public string Msg { get; set; }

       /// <summary>
       /// Gets or sets the data.
       /// </summary>
       /// <value>The data.</value>
       public object Data { get; set; }
         
    }
}
