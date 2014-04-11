using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
   [Export("ReturnPackageManageViewModel", typeof(ReturnPackageManageViewModel))]
    public class ReturnPackageManageViewModel : BindableBase
    {
       public PackageReceiveDto PackageReceiveDto { get; set; }
       private List<SaleRmaDto> _saleRmaList;
       public List<SaleRmaDto> SaleRmaList
       {
           get { return _saleRmaList; }
           set { SetProperty(ref _saleRmaList, value); }
       }

       public RMADto rmaDto;

       public RMADto RmaDto
       {
           get { return rmaDto; }
           set { SetProperty(ref rmaDto, value); }
       }
       private List<RMADto> _rmaDtos;

       public List<RMADto> RamList
       {
           get { return _rmaDtos; }
           set { SetProperty(ref _rmaDtos, value); }
       }

       private List<RmaDetail> rmaDetails;

       public List<RmaDetail> RmaDetailList
       {
           get { return rmaDetails; }
           set { SetProperty(ref rmaDetails, value); }
       }
       public DelegateCommand CommandSearch { get; set; }
       public DelegateCommand CommandGetRmaSaleDetailByRma { get; set; }

       public ReturnPackageManageViewModel()
       {
           CommandSearch = new DelegateCommand(SearchRmaAndSaleRma);
           CommandGetRmaSaleDetailByRma = new DelegateCommand(GetRmaSaleDetailByRma);
           PackageReceiveDto = new PackageReceiveDto();
       }

       public void GetRmaSaleDetailByRma()
       {
           if (rmaDto != null)
           {
               RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(rmaDto.RMANo).ToList();
           }
       }

       private void SearchRmaAndSaleRma()
       {
           RamList = AppEx.Container.GetInstance<IPackageService>().GetRma(PackageReceiveDto).ToList();
           SaleRmaList = AppEx.Container.GetInstance<IPackageService>().GetSaleRma(PackageReceiveDto).ToList();
       }
    }
}
