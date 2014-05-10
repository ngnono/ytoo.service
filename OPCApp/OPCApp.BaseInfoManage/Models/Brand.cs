using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// 品牌
    /// </summary>
    [Uri(Name="brands")]
    public class Brand : Dimension
    {
        private bool enabled;

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否已启用
        /// </summary>
        public bool Enabled 
        {
            get { return enabled; }
            set { SetProperty(ref enabled, value); }
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public Supplier Supplier { get; set; }

        /// <summary>
        /// 销售这个品牌的专柜
        /// </summary>
        public IList<Counter> Counters { get; set; }
    }
}
