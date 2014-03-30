using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("SaleOrderPickupViewModel")]
    public class SaleOrderPickupViewModel : StoreInViewModel
    {
        public SaleOrderPickupViewModel() : base()
        {
            this.SearchSaleStatus=EnumSearchSaleStatus.ShipedSearchStatus;
        }

       
    }
}