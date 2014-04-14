
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
   [Export("WebSiteSalesStatisticsViewModel", typeof(WebSiteSalesStatisticsViewModel))]
    public class WebSiteSalesStatisticsViewModel : BindableBase
    {
        private SearchStatistics _searchCashierDtos;

        public SearchStatistics SearchCashierDto
        {
            get { return _searchCashierDtos; }
            set { SetProperty(ref _searchCashierDtos, value); }
        }
       public WebSiteSalesStatisticsViewModel()
       {
           SearchCashierDto = new SearchStatistics();
           CommandSearch = new DelegateCommand(Search);
           CommandExport = new DelegateCommand(ExportExcel);
       }
       private List<WebSiteSalesStatisticsDto> _webSiteSalesStatisticsDtos;

       public List<WebSiteSalesStatisticsDto> WebSiteSalesStatisticsDtos
       {
           get { return _webSiteSalesStatisticsDtos; }
           set { SetProperty(ref _webSiteSalesStatisticsDtos, value); }
       }
       private void Search()
       {
           WebSiteSalesStatisticsDtos =
               AppEx.Container.GetInstance<IFinancialPayVerify>().GetSalesStatistics(SearchCashierDto).ToList();
       }

       public DelegateCommand CommandSearch { get; set; }

       public DelegateCommand CommandExport { get; set; }

       private void ExportExcel()
       {
           throw new System.NotImplementedException();
       }
    }
}
