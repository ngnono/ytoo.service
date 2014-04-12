using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerReturnGoodsMainViewModel", typeof(CustomerReturnGoodsMainViewModel))]
    public class CustomerReturnGoodsMainViewModel:BindableBase
    {
        private CustomerReturnSearchFinancialViewModel _customerReturnSearchFinancialView;
        [Import]
        public CustomerReturnSearchFinancialViewModel CustomerReturnSearchFinancialViewModel
        {
            get { return _customerReturnSearchFinancialView; }
            set { SetProperty(ref _customerReturnSearchFinancialView, value); }
        }

           private CustomerReturnSearchTransViewModel _customerReturnSearchTransViewModel;
        [Import]
        public CustomerReturnSearchTransViewModel CustomerReturnSearchTransViewModel

         {
            get { return _customerReturnSearchTransViewModel; }
            set { SetProperty(ref _customerReturnSearchTransViewModel, value); }
        }


        private CustomerReturnSearchRmaViewModel _customerReturnSearch;
        [Import]
        public CustomerReturnSearchRmaViewModel CustomerReturnSearchRmaViewModel
        {
            get { return _customerReturnSearch; }
            set { SetProperty(ref _customerReturnSearch, value); }
        }
        public CustomerReturnGoodsMainViewModel()
        {
        }

    }
}
