using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("SaleOrderPickupViewModel")]
    public class SaleOrderPickupViewModel : PrintInvoiceViewModel
    {
        public SaleOrderPickupViewModel() : base()
        {
            SearchSaleStatus = EnumSearchSaleStatus.ShippedSearchStatus;
        }

        protected override void Refresh()
        {
            string salesfilter = string.Format("startdate={0}&enddate={1}&orderno={2}&saleorderno={3}",
               Invoice4Get.StartSellDate.ToShortDateString(),
               Invoice4Get.EndSellDate.ToShortDateString(),
               Invoice4Get.OrderNo, Invoice4Get.SaleOrderNo);
            PageResult<OPC_Sale> re = AppEx.Container.GetInstance<ITransService>().Search(salesfilter, SearchSaleStatus);
            SaleList = re.Result;
            if (InvoiceDetail4List != null) InvoiceDetail4List = new List<OPC_SaleDetail>();
        }
    }
}