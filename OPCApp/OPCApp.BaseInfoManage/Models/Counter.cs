using System;
using System.Collections.Generic;
using Intime.OPC.Modules.Dimension.Common;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// 专柜
    /// </summary>
    [Uri(Name = "counters")]
    public class Counter : Dimension
    {
        private bool repealed;
        private string code;
        private string areaCode;
        private string contactPhoneNumber;

        /// <summary>
        /// 专柜码
        /// </summary>
        public string Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        /// <summary>
        /// 销售区域
        /// </summary>
        public string AreaCode
        {
            get { return areaCode; }
            set { SetProperty(ref areaCode, value); }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhoneNumber
        {
            get { return contactPhoneNumber; }
            set { SetProperty(ref contactPhoneNumber, value); }
        }

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
