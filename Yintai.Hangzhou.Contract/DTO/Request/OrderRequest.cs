using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class OrderRequest : AuthRequest
    {
        public string Order { get; set; }
        public OrderRequestModel OrderModel
        {
            get
            {
                return JsonConvert.DeserializeObject<OrderRequestModel>(Order);
            }
        }

    }
    public class OrderRequestModel:IValidatableObject
    {
        [Required(ErrorMessage="没有选择产品！")]
        public int ProductId { get; set; }

        public string Desc { get; set; }
        [Required(ErrorMessage = "没有选择数量！")]
        [Range(1,5,ErrorMessage="数量不能超过5个")]
        public int Quantity { get; set; }
        
        public IEnumerable<ProductPropertyValueRequest> Properties { get; set; }
        [Required(ErrorMessage = "送货方式没有选择")]
        public ShippingAddressModel ShippingAddress { get; set; }
        [Required(ErrorMessage="支付方式没有选择")]
        public PaymentModel Payment { get; set; }
        public bool NeedInvoice { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceDetail { get; set; }
        public string Memo { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NeedInvoice)
            {
                if (string.IsNullOrEmpty(InvoiceTitle))
                    yield return new ValidationResult("发票抬头不能为空！");
            }
        }

        public string ProductDesc { get {
            if (Properties == null)
                return string.Empty;
           var description = Properties.Aggregate(new StringBuilder(),
                (s, p) => s.AppendFormat("{0}:{1},", p.PropertyName, p.ValueName),
                s => s.ToString());
           if (description.Length > 0)
               description = description.TrimEnd(',');
           return description;
                    
        } }
    }
    public class ProductPropertyValueRequest
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public int? ValueId { get; set; }
        public string ValueName { get; set; }
    }
}
