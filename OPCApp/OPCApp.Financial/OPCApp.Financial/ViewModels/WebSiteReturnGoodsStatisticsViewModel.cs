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
    [Export("WebSiteReturnGoodsStatisticsViewModel", typeof (WebSiteReturnGoodsStatisticsViewModel))]
    public class WebSiteReturnGoodsStatisticsViewModel : BindableBase
    {
        private SearchStatistics _searchCashierDtos;

        private List<WebSiteReturnGoodsStatisticsDto> _webSiteReturnGoodsStatistics;

        public WebSiteReturnGoodsStatisticsViewModel()
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

        public List<WebSiteReturnGoodsStatisticsDto> WebSiteReturnGoodsStatisticsDtos
        {
            get { return _webSiteReturnGoodsStatistics; }
            set { SetProperty(ref _webSiteReturnGoodsStatistics, value); }
        }

        public DelegateCommand CommandSearch { get; set; }

        public DelegateCommand CommandExport { get; set; }

        private void Search()
        {
            WebSiteReturnGoodsStatisticsDtos =
                AppEx.Container.GetInstance<IFinancialPayVerify>().GetReturnGoodsStatistics(SearchCashierDto).ToList();
        }

        private void ExportExcel()
        {
          
        }
    }
}