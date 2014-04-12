using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Customer;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export(typeof(CustomerReturnSearchRmaViewModel))]
    public class CustomerReturnSearchRmaViewModel :BindableBase// CustomerReturnSearchViewModel
    {
        private ReturnGoodsInfoGet returnGoodsInfo;
        public ReturnGoodsInfoGet ReturnGoodsInfoGet
        {
            get { return returnGoodsInfo; }
            set { SetProperty(ref returnGoodsInfo, value); }
        }
        public CustomerReturnSearchRmaViewModel():base()
        {
            ReturnGoodsInfoGet = new ReturnGoodsInfoGet();
        }
    }
}