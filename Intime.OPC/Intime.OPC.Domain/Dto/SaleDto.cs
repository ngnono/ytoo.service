﻿using System;

namespace Intime.OPC.Domain.Dto
{
    public class SaleDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public int SalesType { get; set; }
        public int? ShipViaId { get; set; }
        public int Status { get; set; }
        public string ShippingCode { get; set; }
        public decimal ShippingFee { get; set; }
        public int? ShippingStatus { get; set; }
        public string ShippingStatusName { get; set; }
        public string ShippingRemark { get; set; }
        public DateTime SellDate { get; set; }
        public bool? IfTrans { get; set; }
        public int? TransStatus { get; set; }
        public decimal SalesAmount { get; set; }
        public int? SalesCount { get; set; }
        public int? CashStatus { get; set; }
        public string CashNum { get; set; }
        public DateTime? CashDate { get; set; }
        public int? SectionId { get; set; }
        public int? PrintTimes { get; set; }
        public string Remark { get; set; }
        public DateTime? RemarkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string StatusName { get; set; }

        public string CashStatusName { get; set; }

        public string StoreName { get; set; }

        public string InvoiceSubject { get; set; }
        public string PayType { get; set; }
        public string SectionName { get; set; }

        public string Invoice { get; set; }


        public string StoreTelephone { get; set; }

        public string StoreAddress { get; set; }
        public string TransNo { get; set; }
        
    }
}