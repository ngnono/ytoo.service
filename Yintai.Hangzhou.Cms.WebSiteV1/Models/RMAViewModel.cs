using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class RMAViewModel:BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="退货号")]
        public string RMANo { get; set; }
        [Display(Name = "订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "退货方式")]
        public int RMAType { get; set; }
        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "退货理由")]
        public string Reason { get; set; }
        [Display(Name = "退货金额")]
        public decimal RMAAmount { get; set; }
        [Display(Name = "申请用户")]
        public int CreateUser { get; set; }
        [Display(Name = "申请时间")]
        public System.DateTime CreateDate { get; set; }
        [Display(Name = "操作用户")]
        public int UpdateUser { get; set; }
        [Display(Name = "操作时间")]
        public System.DateTime UpdateDate { get; set; }
        [Display(Name = "退款银行")]
        public string BankName { get; set; }
         [Display(Name = "退款人")]
        public string BankAccount { get; set; }
         [Display(Name = "退款卡号")]
        public string BankCard { get; set; }
        [Display(Name = "不能退货原因")]
        public string RejectReason { get; set; }
        public Nullable<decimal> rebatepostfee { get; set; }
        public Nullable<decimal> chargepostfee { get; set; }
        public Nullable<decimal> ActualAmount { get {
            return RMAAmount - (chargepostfee ?? 0) + (rebatepostfee ?? 0)-(ChargeGiftFee??0);
        } }
        public decimal? ChargeGiftFee { get; set; }
        public string GiftReason { get; set; }
        public string InvoiceReason { get; set; }
        public string RebatePointReason { get; set; }
        public string PostalFeeReason { get; set; }
        public RMAItemViewModel Item { get; set; }
    }
    public class RMAItemViewModel:BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "专柜商品编码")]
        public string StoreItemNo { get; set; }
        [Display(Name = "专柜商品描述")]
        public string StoreItemDesc { get; set; }
        [Display(Name = "数量")]
        public int Quantity { get; set; }
        [Display(Name = "吊牌价")]
        public Nullable<decimal> UnitPrice { get; set; }
        [Display(Name = "购买价")]
        public decimal ItemPrice { get; set; }
        [Display(Name = "总额")]
        public decimal ExtendPrice { get; set; }
       
    }
}