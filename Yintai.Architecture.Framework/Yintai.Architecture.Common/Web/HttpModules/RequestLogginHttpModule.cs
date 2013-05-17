using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Web;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.Extension;

namespace Yintai.Architecture.Common.Web.HttpModules
{
    /// <summary>
    /// CLR Version: 4.0.30319.269
    /// NameSpace: Yintai.Architecture.Framework.Web.HttpModules
    /// FileName: Class1
    ///
    /// Created at 11/8/2012 7:50:21 PM
    /// Description: 
    /// </summary>
    /// <summary>
    /// 请求记录日志
    /// </summary>
    public class RequestLogginHttpModule : IHttpModule
    {
        #region fileds

        private static readonly ILog _log = LoggerManager.Current();

        #endregion

        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new System.EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, System.EventArgs e)
        {
            var context = sender as HttpApplication;

            if (context == null)
            {
                _log.Error("RequestLogginHttpModule Parameter context is null");
                return;
            }
            string form = context.Context.Request.Form.ToJson();
            string query = context.Context.Request.QueryString.ToJson();

            //logging the Request
            var sb = new StringBuilder();

            sb.AppendFormat("{0} [ {1} ] ", context.Context.Request.Url, DateTime.Now);
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("[ Headers ]");
            sb.AppendLine();

            foreach (string key in context.Context.Request.Headers)
            {
                sb.AppendFormat("{0} : {1}", key, context.Context.Request.Headers[key]);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("[ QueryString ]");
            sb.AppendLine();

            foreach (string key in context.Context.Request.QueryString)
            {
                sb.AppendFormat("{0} : {1}", key, context.Context.Request.QueryString[key]);
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("[ Form ]");
            sb.AppendLine();

            foreach (string key in context.Context.Request.Form)
            {
                sb.AppendFormat("{0} : {1}", key, context.Context.Request.Form[key]);
                sb.AppendLine();
            }

            _log.Debug(sb.ToString());
        }

        #region helper

        public Dictionary<string, string> NameValueCollectionToDictionary(NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                dict.Add(key, collection[key]);
            }

            return dict;
        }

        #endregion

        #endregion
    }
}
