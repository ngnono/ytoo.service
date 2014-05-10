using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// 专柜
    /// </summary>
    public class Counter : Dimension
    {
        private bool repealed;

        /// <summary>
        /// 专柜码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 销售区域
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 是否已撤柜
        /// </summary>
        public bool Repealed
        {
            get { return repealed; }
            set { SetProperty(ref repealed, value); }
        }

        /// <summary>
        /// 销售的品牌
        /// </summary>
        public IList<Brand> Brands { get; set; }

        /// <summary>
        /// 管理部门
        /// </summary>
        public Organization Organization { get; set; }
    }
}
