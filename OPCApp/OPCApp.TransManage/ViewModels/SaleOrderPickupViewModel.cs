using System.ComponentModel.Composition;
using OPCAPP.Domain.Enums;

namespace OPCApp.TransManage.ViewModels
{
    [Export("SaleOrderPickupViewModel")]
    public class SaleOrderPickupViewModel : StoreInViewModel
    {
        public SaleOrderPickupViewModel()
        {
            SearchSaleStatus = EnumSearchSaleStatus.ShipedSearchStatus;
        }
    }
}