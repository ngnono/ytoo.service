﻿using System;
using System.Collections.Generic;
using Intime.OPC.Modules.Dimension.Common;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// 品牌
    /// </summary>
    [Uri(Name="brands")]
    public class Brand : Dimension
    {
        private bool enabled;
        private string englishName;
        private string description;

        public Brand()
        {
            Supplier = new Supplier { ID = 4, Name = "SupplierName" };
        }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName
        {
            get { return englishName; }
            set { SetProperty(ref englishName, value); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

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