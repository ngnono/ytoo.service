using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export("ReturnPackageManageViewModel", typeof (ReturnPackageManageViewModel))]
    public class ReturnPackageManageViewModel : BindableBase
    {
        private List<RMADto> _rmaDtos;
        private List<SaleRmaDto> _saleRmaList;
        private List<RmaDetail> rmaDetails;

        public RMADto rmaDto;

        public ReturnPackageManageViewModel()
        {
            CommandSearch = new DelegateCommand(SearchRmaAndSaleRma);
            CommandGetRmaSaleDetailByRma = new DelegateCommand(GetRmaSaleDetailByRma);
            PackageReceiveDto = new PackageReceiveDto();
            CommandSetSaleRmaRemark = new DelegateCommand(SetSaleRmaRemark);
        }

        public void SetSaleRmaRemark()
        {
            //被选择的对象
            string id = RmaDto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.);
        }

        private PackageReceiveDto packageReceiveDto;
        public PackageReceiveDto PackageReceiveDto
        {
            get { return packageReceiveDto; }
            set { SetProperty(ref packageReceiveDto, value); }
        }

        public List<SaleRmaDto> SaleRmaList
        {
            get { return _saleRmaList; }
            set { SetProperty(ref _saleRmaList, value); }
        }

        public RMADto RmaDto
        {
            get { return rmaDto; }
            set { SetProperty(ref rmaDto, value); }
        }

        public List<RMADto> RamList
        {
            get { return _rmaDtos; }
            set { SetProperty(ref _rmaDtos, value); }
        }

        public List<RmaDetail> RmaDetailList
        {
            get { return rmaDetails; }
            set { SetProperty(ref rmaDetails, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandGetRmaSaleDetailByRma { get; set; }
        public DelegateCommand CommandSetSaleRmaRemark { get; set; }

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