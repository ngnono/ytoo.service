using OPCApp.Domain.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OPCApp.Domain.Dto
{
    public class ShippingSaleCreateDto : ValidatableBindableBase
    {
        private string _shippingCode;

        public ShippingSaleCreateDto()
        {
            SaleOrderIDs = new List<string>();
        }

        public string OrderNo { get; set; }

        public string ShippingStatusName { get; set; }

        [Display(Name="快递费")]
        [LocalizedRequired]
        public double ShippingFee { get; set; }

        [Display(Name = "快递单号")]
        [LocalizedRequired]
        public string ShippingCode
        {
            get { return _shippingCode; }
            set { SetProperty(ref _shippingCode,value); }
        }

        [Display(Name = "快递公司")]
        [LocalizedRequired]
        public int? ShipViaID { get; set; }

        public string ShipViaName { get; set; }

        public string RmaNo { get; set; }

        public IList<string> SaleOrderIDs { get; set; }
    }
}