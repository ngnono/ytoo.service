using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    [DataContract]
    public class GetProductInfo4PResponse : BaseResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }


        /// <summary>
        /// 说明
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }


        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember(Name="price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        [DataMember(Name = "originprice")]
        public decimal UnitPrice { get; set; }

        [DataMember(Name="properties")]
        public IEnumerable<ProductPropertyResponse> Properties { get; set; }

        [DataMember(Name="rmapolicy")]
        public string RMAPolicy { get; set; }
        [DataMember(Name="supportpayments")]
        public IEnumerable<PaymentMethodResponse> SupportPayments { get; set; }
        [DataMember(Name="resource")]
        public ResourceInfoResponse Resource { get; set; }
        [DataMember(Name = "dimension")]
        public ResourceInfoResponse DimensionResource { get; set; }
       
    }
    [DataContract]
    public class ProductPropertyResponse
    {
        [DataMember(Name="propertyid")]
        public int PropertyId { get; set; }
         [DataMember(Name = "propertyname")]
        public string PropertyName { get; set; }
         [DataMember(Name = "values")]
        public IEnumerable<ProductPropertyValueReponse> Values { get; set; }
    }
    [DataContract]
    public class ProductPropertyValueReponse
    {
        [DataMember(Name="valueid")]
        public int ValueId { get; set; }
        [DataMember(Name="valuename")]
        public string ValueName { get; set; }
    }
    [DataContract]
    public class PaymentMethodResponse
    {
        [DataMember(Name="code")]
        public string Code { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
