using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Model.Filters
{
    public class ProductFilter
    {
        public DataStatus? DataStatus { get; set; }

        public Timestamp Timestamp { get; set; }

        public string ProductName { get; set; }

        public int? RecommendUser { get; set; }

        public List<int> TagIds { get; set; }

        public int? BrandId { get; set; }

        public int? TopicId { get; set; }

        /// <summary>
        /// 活动Id
        /// </summary>
        public int? PromotionId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (DataStatus != null)
            {
                sb.AppendFormat("ds_", DataStatus);
            }

            if (!String.IsNullOrWhiteSpace(ProductName))
            {
                sb.AppendFormat("pn_", ProductName);
            }



            return sb.ToString();
        }
    }
}
