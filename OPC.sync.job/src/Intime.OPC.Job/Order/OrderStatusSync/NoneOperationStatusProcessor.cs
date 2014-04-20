﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intime.OPC.Job.Order.OrderStatusSync
{
    public class NoneOperationStatusProcessor:AbstractSaleOrderStatusProcessor
    {
        private Domain.Enums.EnumSaleOrderStatus enumSaleOrderStatus;

        public NoneOperationStatusProcessor(Domain.Enums.EnumSaleOrderStatus enumSaleOrderStatus) : base(enumSaleOrderStatus) { }

        public override void Process(string saleOrderNo, Models.OrderStatusResultDto statusResult)
        {
            
        }
    }
}
