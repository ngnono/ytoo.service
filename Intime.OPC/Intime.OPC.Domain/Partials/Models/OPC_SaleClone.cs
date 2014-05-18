using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Base;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Domain.Partials.Models
{
    public partial class OrderClone
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<decimal> RecAmount { get; set; }
        public int Status { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentMethodName { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactPerson { get; set; }
        public string ShippingContactPhone { get; set; }
        public Nullable<bool> NeedInvoice { get; set; }
        public string InvoiceSubject { get; set; }
        public string InvoiceDetail { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        public string ShippingNo { get; set; }
        public Nullable<int> ShippingVia { get; set; }
        public int StoreId { get; set; }
        public int BrandId { get; set; }
        public string Memo { get; set; }
        public Nullable<decimal> InvoiceAmount { get; set; }
        public Nullable<int> TotalPoints { get; set; }
        public string OrderSource { get; set; }
        public Nullable<int> OrderProductType { get; set; }
    }


    public partial class OPC_SaleClone
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public int SalesType { get; set; }
        public Nullable<int> ShipViaId { get; set; }
        public int Status { get; set; }
        public string ShippingCode { get; set; }
        public Nullable<int> ShippingSaleId { get; set; }
        public Nullable<decimal> ShippingFee { get; set; }
        public Nullable<int> ShippingStatus { get; set; }
        public string ShippingRemark { get; set; }
        public System.DateTime SellDate { get; set; }
        public Nullable<bool> IfTrans { get; set; }
        public Nullable<int> TransStatus { get; set; }
        public decimal SalesAmount { get; set; }
        public int SalesCount { get; set; }
        public Nullable<int> CashStatus { get; set; }
        public string CashNum { get; set; }
        public Nullable<System.DateTime> CashDate { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<int> PrintTimes { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> RemarkDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUser { get; set; }

        public StoreClone Store { get; set; }

        public SectionClone Section { get; set; }

        public OrderTransactionClone OrderTransaction { get; set; }

        public OPC_ShippingSaleClone ShippingSale { get; set; }

        public OrderClone Order { get; set; }
    }
}
