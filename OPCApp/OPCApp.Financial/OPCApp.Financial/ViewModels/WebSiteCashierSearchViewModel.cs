using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Financial;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Dto;
using OPCAPP.Domain.Dto.Financial;
using OPCApp.Infrastructure;

namespace OPCApp.Financial.ViewModels
{
    [Export("WebSiteCashierSearchViewModel", typeof (WebSiteCashierSearchViewModel))]
    public class WebSiteCashierSearchViewModel : BindableBase
    {
        private SearchCashierDto _searchCashierDtos;

        private List<WebSiteCashierSearchDto> _webSiteCashierSearchDtos;

        public WebSiteCashierSearchViewModel()
        {
            InitCombo();
            CommandExport = new DelegateCommand(ExportExcel);
            CommandSearch = new DelegateCommand(Search);
        }

        public SearchCashierDto SearchCashierDto
        {
            get { return _searchCashierDtos; }
            set { SetProperty(ref _searchCashierDtos, value); }
        }

        public List<WebSiteCashierSearchDto> WebSiteCashierSearchDtos
        {
            get { return _webSiteCashierSearchDtos; }
            set { SetProperty(ref _webSiteCashierSearchDtos, value); }
        }

        public IList<KeyValue> StoreList { get; set; }


        public IList<KeyValue> PaymentTypeList { get; set; }
        public IList<KeyValue> FinancialTypeList { get; set; }
        public DelegateCommand CommandExport { get; set; }
        public DelegateCommand CommandSearch { get; set; }

        public void InitCombo()
        {
            SearchCashierDto = new SearchCashierDto();
            StoreList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();

            PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
            FinancialTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetFinancialTypeList();
        }

        private void ExportExcel()
        {
            throw new NotImplementedException();
        }

        private void Search()
        {
            
            WebSiteCashierSearchDtos =
                AppEx.Container.GetInstance<IFinancialPayVerify>().GetCashierStatistics(SearchCashierDto).ToList();
        }
    }
}