using Intime.OPC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    queryString.Append(string.Format("{0}={1}&",uriParameter.Name, propertyInfo.GetValue(this).ToString()));
                }
            }
            if (queryString.Length > 0) queryString.Remove(queryString.Length - 1, 1);

            return queryString.ToString();
        }
    }
}
