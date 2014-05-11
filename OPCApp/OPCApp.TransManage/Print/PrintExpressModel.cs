using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace Intime.OPC.Modules.Logistics.Print
{
    public class PrintExpressModel
    {
        /// <summary>
        ///     收货人姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        ///     收货人地址
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        ///     收货人电话
        /// </summary>
        public string CustomerPhone { get; set; }

        public string ExpressFee { get; set; }

        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePerson { get; set; }
        public string StoreTel { get; set; }
    }
}