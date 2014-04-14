using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Customer;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Domain.ReturnGoods;
using OPCApp.Infrastructure;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ReturnPackagePrintExpressViewModel))]
    public class ReturnPackagePrintExpressViewModel : BindableBase
    {
        private List<RmaDetail> _rmaDetails;
        private RMADto _rmaDto;
        private List<RMADto> _rmaDtoList;
        private RmaExpressDto _rmaExpressDto;
        private RmaExpressSaveDto _rmaExpressSave;
        private List<OPC_ShippingSale> _shipList;
        private OPC_ShippingSale _shipSale;

        public ReturnPackagePrintExpressViewModel()
        {
            RmaExpressDto = new RmaExpressDto();
            RmaExpressSaveDto = new RmaExpressSaveDto();
            CommandGetRmaDetailByRmaNo = new DelegateCommand(GetRmaDetailByRmaNo);
            CommandGetRmaByRmaNo = new DelegateCommand(GetRmaByRmaNo);
            CommandSearch = new DelegateCommand(SearchShip);
            CommandPrintView = new DelegateCommand(PrintView);
            CommandOnlyPrint = new DelegateCommand(OnlyPrint);
            CommandPrintExpress = new DelegateCommand(PrintExpressComplete);
            CommandSaveShip = new DelegateCommand(SaveShip);
            CommandSetShippingRemark = new DelegateCommand(SetShippingRemark);
            CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);

            InitCombo();
        }

        public OPC_ShippingSale ShipSaleSelected
        {
            get { return _shipSale; }
            set { SetProperty(ref _shipSale, value); }
        }

        public ShipVia ShipVia { get; set; }
        public List<ShipVia> ShipViaList { get; set; }

        public List<OPC_ShippingSale> ShipSaleList
        {
            get { return _shipList; }
            set { SetProperty(ref _shipList, value); }
        }

        public DelegateCommand CommandSetRmaRemark { get; set; }

        public DelegateCommand CommandGetRmaByRmaNo { get; set; }

        public DelegateCommand CommandSetShippingRemark { get; set; }

        //快递单备注

        public DelegateCommand CommandSaveShip { get; set; }

        public DelegateCommand CommandPrintExpress { get; set; }

        public DelegateCommand CommandOnlyPrint { get; set; }

        public DelegateCommand CommandPrintView { get; set; }

        public DelegateCommand CommandSearch { get; set; }

        public RMADto RMADto
        {
            get { return _rmaDto; }
            set { SetProperty(ref _rmaDto, value); }
        }


        public List<RmaDetail> RmaDetailList
        {
            get { return _rmaDetails; }
            set { SetProperty(ref _rmaDetails, value); }
        }

        public List<RMADto> RMADtoList
        {
            get { return _rmaDtoList; }
            set { SetProperty(ref _rmaDtoList, value); }
        }


        public DelegateCommand CommandSearchRmaDtoInfo { get; set; }
        public DelegateCommand CommandGetRmaDetailByRmaNo { get; set; }

        public RmaExpressDto RmaExpressDto
        {
            get { return _rmaExpressDto; }
            set { SetProperty(ref _rmaExpressDto, value); }
        }

        public RmaExpressSaveDto RmaExpressSaveDto
        {
            get { return _rmaExpressSave; }
            set { SetProperty(ref _rmaExpressSave, value); }
        }

        public void SetRmaRemark()
        {
            string id = RMADto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
        }

        private void GetRmaByRmaNo()
        {
            if (ShipSaleSelected != null)
            {
                ClearRmaData();
                RMADtoList =
                    AppEx.Container.GetInstance<IPackageService>()
                        .GetRmaForPrintExpress(ShipSaleSelected.RmaNo)
                        .ToList();
            }
        }

        private void ClearRmaData()
        {
            RmaDetailList.Clear();
            RMADtoList.Clear();
        }

        private void SearchShip()
        {
            ClearRmaData();
            ShipSaleList =
                AppEx.Container.GetInstance<IPackageService>().GetShipListWithReturnGoods(RmaExpressDto).ToList();
        }

        private void SetShippingRemark()
        {
            //被选择的对象
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            string id = ShipSaleSelected.ExpressCode;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark); //填写的是快递单
        }

        public void SaveShip()
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            try
            {
                RMADtoList =
                    AppEx.Container.GetInstance<IPackageService>()
                        .GetRmaForPrintExpress(ShipSaleSelected.RmaNo)
                        .ToList();
            }
            catch
            {
            }
        }

        public void PrintExpressComplete() //
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            if (string.IsNullOrEmpty(ShipSaleSelected.RmaNo))
            {
                MessageBox.Show("请先保存快递信息", "提示");
                return;
            }
            bool falg = AppEx.Container.GetInstance<IPackageService>().ShipPrintComplete(ShipSaleSelected.ExpressCode);
            MessageBox.Show(falg ? "操作成功" : "操作失败", "提示");
            if (falg)
            {
                ClearRmaData();
            }
        }

        public void OnlyPrint()
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            if (string.IsNullOrEmpty(ShipSaleSelected.RmaNo))
            {
                MessageBox.Show("请先保存快递信息", "提示");
                return;
            }
            bool falg = AppEx.Container.GetInstance<IPackageService>().ShipPrint(ShipSaleSelected.ExpressCode);
            MessageBox.Show(falg ? "操作成功" : "操作失败", "提示");
        }

        public void PrintView()
        {
        }

        public void InitCombo()
        {
            ShipViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
        }

        public void GetRmaDetailByRmaNo()
        {
            if (RMADto != null)
            {
                RmaDetailList = AppEx.Container.GetInstance<ICustomerReturnSearch>()
                    .GetRmaDetailByRmaNo(RMADto.RMANo).ToList();
            }
        }
    }
}