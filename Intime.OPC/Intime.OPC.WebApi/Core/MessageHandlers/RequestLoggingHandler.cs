﻿using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intime.OPC.WebApi.Core.MessageHandlers
{
    public class RequestLoggingHandler : DelegatingHandler
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(RequestLoggingHandler));

        /// <summary>
        /// 是否开启 request logging
        /// </summary>
        private bool Enabled
        {
            get
            {
                var enable = ConfigurationManager.AppSettings["RequestLogging:Enabled"];

                if (enable == null)
                {
                    return false;
                }

                bool result;

                bool.TryParse(enable, out result);

                return result;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!Enabled)
            {
                return base.SendAsync(request, cancellationToken);
            }

            //只有DEBUG

            var sb = new StringBuilder();
            sb.AppendLine();

            //method
            //sb.AppendLine("*************common start*************");

            //sb.AppendLine("*************common end*************");

            //header
            var header = request.Headers;
            if (header != null)
            {
                sb.AppendLine("*************header start*************");
                sb.Append("header:[");

                foreach (var httpRequestHeader in header)
                {
                    sb.AppendFormat("{{{0}:", httpRequestHeader.Key);
                    foreach (var item in httpRequestHeader.Value)
                    {
                        sb.Append(item);
                        sb.Append(",");
                    }
                    sb.Append("},");
                }

                sb.AppendLine("]");

                sb.AppendLine("*************header end*************");
            }
            //querystring

            var query = request.GetQueryNameValuePairs();
            if (query != null)
            {
                sb.AppendLine("*************query start*************");
                sb.AppendLine(String.Format("http url:{0}", request.RequestUri));
                sb.AppendLine(String.Format("http method:{0}", request.Method));
                sb.Append("query:[");
                foreach (var q in query)
                {
                    sb.AppendFormat("{0}:{1},", q.Key, q.Value);
                }

                sb.AppendLine("]");

                sb.AppendLine("*************query end*************");
            }

            //form

            var form = request.Headers.From;

            if (form != null)
            {
                sb.AppendLine("*************from start*************");
                sb.Append("form:[");
                sb.Append(form);
                sb.AppendLine("]");
                sb.AppendLine("*************from end*************");
            }

            //content
            if (request.Content != null)
            {
                sb.AppendLine("*************content start*************");
                sb.Append("mediatype:[");
                sb.Append(request.Content.Headers.ContentType.MediaType);
                sb.Append("],");

                sb.AppendLine();
                sb.Append("content:[");
                sb.Append(request.Content.ReadAsStringAsync().Result);
                sb.AppendLine("]");
                sb.AppendLine("*************content end*************");
            }

            _log.Debug(sb.ToString());

            return base.SendAsync(request, cancellationToken);
        }
    }
}
