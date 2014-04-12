using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerReturnGoodsMainViewModel", typeof(CustomerReturnGoodsMainViewModel))]
    public class CustomerReturnGoodsMainViewModel : BindableBase
    {
        [Import]
        public CustomerReturnSearchRmaViewModel CustomerReturnSearchRmaViewModel;
        [Import]
        public CustomerReturnSearchFinancialViewModel CustomerReturnSearchFinancialViewModel;
        [Import]
        public CustomerReturnSearchTransViewModel CustomerReturnSearchTransViewModel;
      
    }
}
