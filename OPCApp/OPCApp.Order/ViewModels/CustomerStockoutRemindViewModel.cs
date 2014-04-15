using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerReturnGoodsMainViewModel", typeof(CustomerReturnGoodsMainViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public  class CustomerStockoutRemindViewModel:BindableBase
    {
        private CustomerStockoutRemindCommonViewModel _customerStockoutRemindCommonViewModel;

        [Import]
        public CustomerStockoutRemindCommonViewModel CustomerReturnSearchFinancialViewModel
        {
            get { return _customerStockoutRemindCommonViewModel; }
            set { SetProperty(ref _customerStockoutRemindCommonViewModel, value); }
        }
    }
}
