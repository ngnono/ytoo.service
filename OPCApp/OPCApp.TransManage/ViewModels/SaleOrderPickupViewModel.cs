using System.ComponentModel.Composition;
using OPCApp.Domain.Enums;

namespace OPCApp.TransManage.ViewModels
{
    [Export("SaleOrderPickupViewModel")]
    public class SaleOrderPickupViewModel : StoreInViewModel
    {
        public override string GetFilter()
        {
            return string.Format("startdate={0}&enddate={1}&orderno={2}&saleorderno={3}&pageIndex={4}&pageSize={5}",
          Invoice4Get.StartSellDate.ToShortDateString(),
          Invoice4Get.EndSellDate.ToShortDateString(),
          Invoice4Get.OrderNo,Invoice4Get.SaleOrderNo, 1, 50);
        }
        public SaleOrderPickupViewModel()
        {

            SearchSaleStatus = EnumSearchSaleStatus.ShipedSearchStatus;
        }
    }
}