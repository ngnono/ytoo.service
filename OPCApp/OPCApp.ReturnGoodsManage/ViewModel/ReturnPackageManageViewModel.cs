using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.Domain.Customer;
using Microsoft.Practices.Prism.Mvvm;
namespace OPCApp.ReturnGoodsManage.ViewModel
{
   [Export("ReturnPackageManageViewModel", typeof(ReturnPackageManageViewModel))]
    public class ReturnPackageManageViewModel : BindableBase
    {
       public PackageReceiveDto PackageReceiveDto { get; set; }
       private List<SaleRmaDto> _saleRmaDtos;
       public List<SaleRmaDto> SaleRmaDtos
       {
           get { return _saleRmaDtos; }
           set { SetProperty(ref _saleRmaDtos, value); }
       }

       public ReturnPackageManageViewModel()
       {
           PackageReceiveDto=new  PackageReceiveDto();
       }
    }
}
