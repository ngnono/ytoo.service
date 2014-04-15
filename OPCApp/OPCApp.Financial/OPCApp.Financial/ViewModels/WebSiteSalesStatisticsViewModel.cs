using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Financial;
using OPCAPP.Domain.Dto.Financial;
using OPCApp.Infrastructure;

namespace OPCApp.Financial.ViewModels
{
    [Export("WebSiteSalesStatisticsViewModel", typeof (WebSiteSalesStatisticsViewModel))]
    public class WebSiteSalesStatisticsViewModel : BindableBase
    {
        private SearchStatistics _searchCashierDtos;

        private List<WebSiteSalesStatisticsDto> _webSiteSalesStatisticsDtos;

        public WebSiteSalesStatisticsViewModel()
        {
            SearchCashierDto = new SearchStatistics();
            CommandSearch = new DelegateCommand(Search);
            CommandExport = new DelegateCommand(ExportExcel);
        }

        public SearchStatistics SearchCashierDto
        {
            get { return _searchCashierDtos; }
            set { SetProperty(ref _searchCashierDtos, value); }
        }

        public List<WebSiteSalesStatisticsDto> WebSiteSalesStatisticsDtos
        {
            get { return _webSiteSalesStatisticsDtos; }
            set { SetProperty(ref _webSiteSalesStatisticsDtos, value); }
        }

        public DelegateCommand CommandSearch { get; set; }

        public DelegateCommand CommandExport { get; set; }

        private void Search()
        {
            WebSiteSalesStatisticsDtos =
                AppEx.Container.GetInstance<IFinancialPayVerify>().GetSalesStatistics(SearchCashierDto).ToList();
        }

        private void ExportExcel()
        {
           
        }
    }
}