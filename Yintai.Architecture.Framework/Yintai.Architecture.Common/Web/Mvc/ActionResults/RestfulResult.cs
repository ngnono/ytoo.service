using System;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Yintai.Architecture.Common.Web.Mvc.ActionResults
{
    public class RestfulResult : ActionResult
    {
        #region .ctor

        #endregion

        #region Properties

        /// <summary>
        /// Result中的数据
        /// </summary>
        public Object Data { get; set; }

        #endregion

        #region Methods

        public override void ExecuteResult(ControllerContext context)
        {
            var encoding = Encoding.UTF8;

            var dcs = new DataContractJsonSerializer(Data.GetType());

            //application/json text/x-json text/html  text/xml

            var format = context.HttpContext.Request[Define.Format];

            if (String.IsNullOrEmpty(format))
            {
                format = Define.Json; // 如果为空，将会使用默认值
            }

            switch (format.ToLower())
            {
                case Define.Json:
                    context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
                    break;
                case Define.Xml:
                    context.HttpContext.Response.ContentType = "text/xml; charset=utf-8";
                    break;
                default:
                    context.HttpContext.Response.ContentType = "text/html; charset=utf-8";
                    break;
            }

            using (XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(context.HttpContext.Response.OutputStream, encoding))
            {
                dcs.WriteObject(writer, Data);
                writer.Flush();
            }
        }


        #endregion
    }
}
