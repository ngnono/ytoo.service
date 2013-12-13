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
        [Required(ErrorMessage = "不能退货原因不能为空")]
        public string RejectReason { get; set; }
        [Display(Name="联系电话")]
        public string ContactPhone { get; set; }
         [Display(Name = "退邮费")]
        public Nullable<decimal> rebatepostfee { get; set; }
         [Display(Name = "收邮费")]
        public Nullable<decimal> chargepostfee { get; set; }
         [Display(Name = "实际退款")]
        public Nullable<decimal> ActualAmount { get {
            return RMAAmount - (chargepostfee ?? 0) + (rebatepostfee ?? 0)-(ChargeGiftFee??0);
        } }
         [Display(Name = "赠品扣款")]
        public decimal? ChargeGiftFee { get; set; }
         [Display(Name = "赠品处理")]
        public string GiftReason { get; set; }
         [Display(Name = "发票处理")]
        public string InvoiceReason { get; set; }
         [Display(Name = "积点处理")]
        public string RebatePointReason { get; set; }
         [Display(Name = "邮费处理")]
        public string PostalFeeReason { get; set; }
        public RMAItemViewModel Item { get; set; }
        public IEnumerable<RMALogViewModel> Logs { get; set; }
    }
    public class RMAItemViewModel:BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "专柜商品编码")]
        public string StoreItem { get; set; }
        [Display(Name = "专柜商品描述")]
        public string StoreDesc { get; set; }
        [Display(Name = "数量")]
        public int Quantity { get; set; }
        [Display(Name = "吊牌价")]
        public Nullable<decimal> UnitPrice { get; set; }
        [Display(Name = "购买价")]
        public decimal ItemPrice { get; set; }
        [Display(Name = "总额")]
        public decimal ExtendPrice { get; set; }
         [Display(Name = "营业员")]
        public string SalesPerson { get; set; }
         [Display(Name = "商品编码")]
         public string ProductId { get; set; }
         [Display(Name = "显示名称")]
         public string ProductName { get; set; }
       
    }

    public class RMALogViewModel : BaseViewModel {
        public int Id { get; set; }
        public string RMANo { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string Operation { get; set; }
    }
}