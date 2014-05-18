using Intime.OPC.Infrastructure;
using OPCApp.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Infrastructure.Service
{
    public class QueryCriteria : IQueryCriteria
    {
        [UriParameter("page")]
        public int PageIndex { get; set; }

        [UriParameter("pagesize")]
        public int PageSize { get; set; }

        public string BuildQueryString()
        {
            var queryString = new StringBuilder();
            var properties = this.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(UriParameterAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    var uriParameter = (UriParameterAttribute)attributes[0];
                    var parameterValue = GetParameterValue(propertyInfo);

                    queryString.Append(string.Format("{0}={1}&", uriParameter.Name, parameterValue.ToString()));
                }
            }
            if (queryString.Length > 0) queryString.Remove(queryString.Length - 1, 1);

            return queryString.ToString();
        }

        private string GetParameterValue(PropertyInfo propertyInfo)
        {
            var propertyValue = propertyInfo.GetValue(this);

            if (propertyValue == null)
            {
                return string.Empty;
            }

            if (propertyValue.GetType().IsEnum)
            {
                propertyValue = (int)propertyValue;
            }

            if (propertyValue.GetType().Equals(typeof(DateTime)))
            {
                propertyValue = ((DateTime)propertyValue).ToShortDateString();
            }

            return propertyValue.ToString();
        }
    }
}
