
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;

using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.Financial;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Dto;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.Interface.Financial;

namespace OPCApp.Financial.ViewModels
{
   [Export("WebSiteCashierSearchViewModel", typeof(WebSiteCashierSearchViewModel))]
    public class WebSiteCashierSearchViewModel : BindableBase
   {
       private SearchCashierDto _searchCashierDtos;

       public SearchCashierDto SearchCashierDto
       {
           get { return _searchCashierDtos; }
           set { SetProperty(ref _searchCashierDtos, value); }
       }
       private List<WebSiteCashierSearchDto> _webSiteCashierSearchDtos;

       public List<WebSiteCashierSearchDto> WebSiteCashierSearchDtos
       {
           get { return _webSiteCashierSearchDtos; }
           set { SetProperty(ref _webSiteCashierSearchDtos, value); }
       }
       public IList<KeyValue> StoreList { get; set; }


       public IList<KeyValue> PaymentTypeList { get; set; }
       public IList<KeyValue> FinancialTypeList { get; set; }
       public void InitCombo()
       {
           // OderStatusList=new 
           StoreList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();

           PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
           FinancialTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetFinancialTypeList();
       }
       public WebSiteCashierSearchViewModel()
       {
           InitCombo();
           CommandExport = new DelegateCommand(ExportExcel);
           CommandSearch = new DelegateCommand(Search);
       }

       private void ExportExcel()
       {
           throw new System.NotImplementedException();
       }

       public DelegateCommand CommandExport { get; set; }

       private void Search()
       {
           WebSiteCashierSearchDtos = AppEx.Container.GetInstance<IFinancialPayVerify>().GetCashierStatistics(SearchCashierDto).ToList();
       }

       public DelegateCommand CommandSearch { get; set; }

   }
}
