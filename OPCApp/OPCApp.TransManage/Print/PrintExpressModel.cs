using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace OPCApp.TransManage.Print
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
    }
}