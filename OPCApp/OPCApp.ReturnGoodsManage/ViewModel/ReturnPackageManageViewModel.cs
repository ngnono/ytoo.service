using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ReturnPackageManageViewModel))]
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
            CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);
            CommandReceivingGoodsSubmit = new DelegateCommand(ReceivingGoodsSubmit);
        }
        //退货单备注
        public void SetRmaRemark()
        {
            string id = RmaDto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
        }
        //收货单备注
        public void SetSaleRmaRemark()
        {
            string id = SaleRma.RmaNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetSaleRMARemark);
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

        private SaleRmaDto _saleRma;
        public SaleRmaDto SaleRma
        {
            get { return _saleRma; }
            set { SetProperty(ref _saleRma, value); }
        }
        public RMADto RmaDto
        {
            get { return rmaDto; }
            set { SetProperty(ref rmaDto, value); }
        }

        public List<RMADto> RmaList
        {
            get { return _rmaDtos; }
            set { SetProperty(ref _rmaDtos, value); }
        }

        public List<RmaDetail> RmaDetailList
        {
            get { return rmaDetails; }
            set { SetProperty(ref rmaDetails, value); }
        }

        public DelegateCommand CommandSetSaleRmaRemark { get; set; }
        public DelegateCommand CommandReceivingGoodsSubmit { get; set; }
        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandGetRmaSaleDetailByRma { get; set; }
        public DelegateCommand CommandSetRmaRemark { get; set; }

        public void GetRmaSaleDetailByRma()
        {
            if (rmaDto != null)
            {
                RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(rmaDto.RMANo).ToList();
            }
        }

        public void ReceivingGoodsSubmit()
        {
            var saleRmaSelected = SaleRmaList.Where(e => e.IsSelected).ToList();
            if (saleRmaSelected.Count==0)
            {
                MessageBox.Show("请勾选收货单", "提示");
                return;
            }
            bool flag =
                AppEx.Container.GetInstance<IPackageService>()
                    .ReceivingGoodsSubmit(saleRmaSelected.Select(e => e.RmaNo).ToList());
            MessageBox.Show(flag ? "确认收货成功" : "确认收货失败", "提示");
            if (flag)
            {
                RmaDetailList.Clear();
                SearchRmaAndSaleRma();
            }
        }

        private void SearchRmaAndSaleRma()
        {
            RmaList = AppEx.Container.GetInstance<IPackageService>().GetRma(PackageReceiveDto).ToList();
            SaleRmaList = AppEx.Container.GetInstance<IPackageService>().GetSaleRma(PackageReceiveDto).ToList();
        }
    }
}