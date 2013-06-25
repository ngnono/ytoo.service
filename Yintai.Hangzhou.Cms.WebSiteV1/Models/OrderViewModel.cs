using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Service.Contract;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class OrderViewModel : BaseViewModel,IValidatableObject
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "客户编号")]
        public int CustomerId { get; set; }
        [Display(Name = "订单金额")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "已支付金额")]
        public Nullable<decimal> RecAmount { get; set; }
        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "支付方式代码")]
        public string PaymentMethodCode { get; set; }
        [Display(Name = "支付方式")]
        public string PaymentMethodName { get; set; }
        [Display(Name = "送货邮编")]
        public string ShippingZipCode { get; set; }
        [Display(Name = "送货地址")]
        public string ShippingAddress { get; set; }
        [Display(Name = "送货联系人")]
        public string ShippingContactPerson { get; set; }
        [Display(Name = "送货联系手机")]
        public string ShippingContactPhone { get; set; }
        [Display(Name = "是否开发票")]
        public Nullable<bool> NeedInvoice { get; set; }
        [Display(Name = "发票抬头")]
        public string InvoiceSubject { get; set; }
        [Display(Name = "发票明细")]
        public string InvoiceDetail { get; set; }
        [Display(Name = "运费")]
        public Nullable<decimal> ShippingFee { get; set; }
        [Display(Name = "创建日期")]
        public System.DateTime CreateDate { get; set; }
        [Display(Name = "创建用户编码")]
        public int CreateUser { get; set; }
        [Display(Name="更新日期")]
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        [Display(Name = "运单号")]
        public string ShippingNo { get; set; }
        [Display(Name = "配送方式编码")]
        public Nullable<int> ShippingVia { get; set; }
        [Display(Name = "所属门店编码")]
        public int StoreId { get; set; }
        [Display(Name = "所属门店")]
        public string StoreName { get; set; }
        [Display(Name = "客户")]
        public CustomerViewModel Customer { get; set; }
        [Display(Name = "配送方式")]
        public ShipViaEntity ShippingViaMethod { get; set; }
        [Display(Name = "商品明细")]
        public IEnumerable<OrderItemViewModel> Items { get; set; }

        public string StatusName {
            get {
                return ((OrderStatus)Status).ToFriendlyString();
            }
        }
        public string RMAStatusName {
            get {
                if (RMAs == null ||RMAs.Count()<=0)
                    return string.Empty;
                return ((RMAStatus)RMAs.First().Status).ToFriendlyString();
            }
        }

        public IEnumerable<OrderLogViewModel> Logs { get; set; }

        public IEnumerable<RMAViewModel> RMAs { get; set; }

        public bool CanVoid { get {
            return Status == (int)OrderStatus.Create ||
                Status == (int)OrderStatus.CustomerConfirmed ||
                Status == (int)OrderStatus.AgentConfirmed||
                Status==(int)OrderStatus.OrderPrinted;
        } }
       

        public bool CanChangeStoreItem { get {
            return Status == (int)OrderStatus.Create ||
                Status == (int)OrderStatus.AgentConfirmed;
        } }


        public bool CanShipping { get {
            return Status == (int)OrderStatus.PreparePack;
        } }

        public bool CanPrint { get {
            return Status == (int)OrderStatus.AgentConfirmed;
        } }

        public bool CanPrintShipping { get {
            return Status == (int)OrderStatus.OrderPrinted;
        } }

        public bool CanChangeReceived { get {
            return Status == (int)OrderStatus.Shipped;
        } }

        public bool CanPrintSales { get {
            return Status == (int)OrderStatus.CustomerReceived;
        } }

        public bool CanRebate {
            get
            {
                return Status == (int)OrderStatus.Convert2Sales;
            }
        }

        public static IEnumerable<SelectListItem> SelectedShipping { get {
            var shipping = ServiceLocator.Current.Resolve<IShippViaRepository>();
           return shipping.Get(s => s.Status != (int)DataStatus.Deleted).ToList().Select(s=>new SelectListItem() { 
             Text = s.Name,
              Value = s.Id.ToString()
           });
        } }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           if (Status ==(int)OrderStatus.Shipped)
            {
                if (!ShippingVia.HasValue)
                    yield return new ValidationResult("配送方式需要设置！");
               if (string.IsNullOrEmpty(ShippingNo))
                   yield return new ValidationResult("快递单号需要设置！");
            }
        }

        public bool CanChangeReject { get {
            return Status == (int)OrderStatus.Shipped;
        } }

        public bool CanCreateOffRMA { get {
            if (Status == (int)OrderStatus.Convert2Sales &&
                (RMAs == null || !RMAs.Any(r => r.Status == (int)RMAStatus.Created || r.Status==(int)RMAStatus.PrintRMA||r.Status==(int)RMAStatus.Reject2Customer)))
                return true;
            return false;
        } }

        public bool CanPrintRMA { get {
            if (RMAs != null && RMAs.Any(r => r.Status == (int)RMAStatus.Created))
                return true;
            return false;
        }}

        public bool CanVoidRMA { get {
            if (RMAs != null && RMAs.Any(r => r.Status == (int)RMAStatus.Created))
                return true;
            return false;
        } }

        public RMAViewModel FirstActiveRMA { get {
            if (RMAs == null||RMAs.Count()==0)
                return null;
            return RMAs.Where(r => r.Status != (int)RMAStatus.Void).OrderByDescending(r => r.CreateDate).First();
        } }

        public string ShippingViaMethod_Name { get; set; }

        public static bool IsAuthorized(int storeId, int brandId,out string error)
        { 
            string errorUnAuthorizedDataAccess = "没有授权操作该订单！";
            error = string.Empty;
            var currentUser = ServiceLocator.Current.Resolve<IAuthenticationService>().CurrentUserFromHttpContext(HttpContext.Current);
            if (currentUser == null)
            {
                error =errorUnAuthorizedDataAccess;
                return false;
            }
            IUserAuthRepository authRepo = ServiceLocator.Current.Resolve<IUserAuthRepository>();
            if (currentUser.Role == (int)UserRole.Admin)
                return true;
            if (!authRepo.Get(a => a.UserId == currentUser.CustomerId)
                .Any(a => a.StoreId == 0 || (a.StoreId == storeId &&
                         (a.BrandId == 0 || a.BrandId == brandId))))
            {
                error = errorUnAuthorizedDataAccess;
                return false;
            }
            return true;

        }
    }

    public class OrderItemViewModel : BaseViewModel,IValidatableObject
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "商品编码")]
        public int ProductId { get; set; }
        [Display(Name = "订购描述")]
        public string ProductDesc { get; set; }
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
        [Display(Name = "品牌编码")]
        public int BrandId { get; set; }
        [Display(Name = "门店编码")]
        public int StoreId { get; set; }
        [Display(Name = "商品图片")]
        public ResourceViewModel ProductResource { get; set; }
        [Display(Name = "品牌")]
        public BrandViewModel Brand { get; set; }
        [Display(Name = "门店")]
        public StoreViewModel Store { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(StoreItemNo))
                yield return new ValidationResult("专柜商品编码没有设置！");
            if (string.IsNullOrEmpty(StoreItemDesc))
                yield return new ValidationResult("专柜商品描述没有设置！");
        }
    }

    public class OrderLogViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "操作者")]
        public int CustomerId { get; set; }
        [Display(Name = "创建时间")]
        public System.DateTime CreateDate { get; set; }
        [Display(Name = "操作")]
        public string Operation { get; set; }
        [Display(Name = "操作类型")]
        public int Type { get; set; }
    }

    public class InboundPackViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        public string SourceNo { get; set; }
        public int SourceType { get; set; }
        [Display(Name = "配送方式")]
        public int ShippingVia { get; set; }
        [Display(Name = "运单号")]
        [Required]
        public string ShippingNo { get; set; }

    }

    public class ConfirmStoreItemViewModel
    {
        public IEnumerable<OrderItemViewModel> Items { get; set; }
    }

}
