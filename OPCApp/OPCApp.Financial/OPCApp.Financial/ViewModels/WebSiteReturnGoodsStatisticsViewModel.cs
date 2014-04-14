
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
   [Export("WebSiteReturnGoodsStatisticsViewModel", typeof(WebSiteReturnGoodsStatisticsViewModel))]
    public class WebSiteReturnGoodsStatisticsViewModel : BindableBase
    {
       private SearchStatistics _searchCashierDtos;

       public SearchStatistics SearchCashierDto
        {
            get { return _searchCashierDtos; }
            set { SetProperty(ref _searchCashierDtos, value); }
        }
       public WebSiteReturnGoodsStatisticsViewModel()
       {
           SearchCashierDto = new SearchStatistics();
           CommandSearch = new DelegateCommand(Search);
           CommandExport = new DelegateCommand(ExportExcel);
       }
       private List<WebSiteReturnGoodsStatisticsDto> _webSiteReturnGoodsStatistics;

       public List<WebSiteReturnGoodsStatisticsDto> WebSiteReturnGoodsStatisticsDtos
       {
           get { return _webSiteReturnGoodsStatistics; }
           set { SetProperty(ref _webSiteReturnGoodsStatistics, value); }
       }
       private void Search()
       {
           WebSiteReturnGoodsStatisticsDtos =
               AppEx.Container.GetInstance<IFinancialPayVerify>().GetReturnGoodsStatistics(SearchCashierDto).ToList();
       }

       public DelegateCommand CommandSearch { get; set; }

       public DelegateCommand CommandExport { get; set; }

       private void ExportExcel()
       {
           throw new System.NotImplementedException();
       }
    }
}
