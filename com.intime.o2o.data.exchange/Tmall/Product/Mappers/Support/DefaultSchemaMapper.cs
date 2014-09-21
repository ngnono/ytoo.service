using System;
using System.Collections;
using System.IO;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers;
using Commons.Collections;

using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace com.intime.o2o.data.exchange.Tmall.Mappers.Support
{
    /// <summary>
    /// 默认Schema转化器
    /// </summary>
    public class DefaultSchemaMapper : ISchemaMapper
    {
        private readonly VelocityEngine _velocity;
        private readonly string _tplPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\tmall");

        public DefaultSchemaMapper()
        {
            var props = new ExtendedProperties();
            props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, _tplPath);
            props.SetProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            props.SetProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");

            _velocity = new VelocityEngine(props);
        }

        public string Map(string schemaName, Hashtable context)
        {
            var template = _velocity.GetTemplate(string.Format("{0}.xml", schemaName));

            var velocityContext = new VelocityContext(context);
            var writer = new StringWriter();
            template.Merge(velocityContext, writer);

            return writer.GetStringBuilder().ToString();
        }
    }
}
