using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intime.OPC.Infrastructure.Service;
using Intime.OPC.Infrastructure.Validation;

namespace OPCApp.Domain.Models
{
    /// <summary>
    /// 专柜
    /// </summary>
    [Uri("counters")]
    public class Counter : Dimension
    {
        private bool _repealed;
        private string _code;
        private string _areaCode;
        private string _contactPhoneNumber;

        /// <summary>
        /// 专柜码
        /// </summary>
        [Display(Name = "专柜码")]
        [LocalizedRequired()]
        [LocalizedMaxLength(200)]
        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        /// <summary>
        /// 销售区域
        /// </summary>
        [Display(Name = "销售区域")]
        [LocalizedMaxLength(200)]
        public string AreaCode
        {
            get { return _areaCode; }
            set { SetProperty(ref _areaCode, value); }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        [LocalizedMaxLength(50)]
        public string ContactPhoneNumber
        {
            get { return _contactPhoneNumber; }
            set { SetProperty(ref _contactPhoneNumber, value); }
        }

        /// <summary>
        /// 是否已撤柜
        /// </summary>
        public bool Repealed
        {
            get { return _repealed; }
            set { SetProperty(ref _repealed, value); }
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
