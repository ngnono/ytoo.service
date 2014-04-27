using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Financial;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Dto;
using OPCApp.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.Financial.ViewModels
{
    [Export("ReturnPackageManageViewModel", typeof (ReturnGoodsPaymentVerifyViewModel))]
    public class ReturnGoodsPaymentVerifyViewModel : BindableBase
    {
        private List<RMADto> _rmaDtos;
        private List<SaleRmaDto> _saleRmaList;
        private ReturnGoodsPayDto returnGoodsDto;
        public decimal rmaDecimal;
        private List<RmaDetail> rmaDetails;
        public RMADto rmaDto;
        private SaleRmaDto saleRma;

        public ReturnGoodsPaymentVerifyViewModel()
        {
            CommandSearch = new DelegateCommand(SearchSaleRma);
            CommandGetRmaSaleDetailByRma = new DelegateCommand(GetRmaSaleDetailByRma);
            ReturnGoodsPayDto = new ReturnGoodsPayDto();
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandGetRmaByOrder = new DelegateCommand(GetRmaByOrder);
            CommandGetRmaDetailByRma = new DelegateCommand(GetRmaDetailByRma);
            CommandSetRmaRemark = new DelegateCommand(SetRmaRemark);
            InitCombo();
        }

        public ReturnGoodsPayDto ReturnGoodsPayDto
        {
            get { return returnGoodsDto; }
            set { SetProperty(ref returnGoodsDto, value); }
        }

        public List<SaleRmaDto> SaleRmaList
        {
            get { return _saleRmaList; }
            set { SetProperty(ref _saleRmaList, value); }
        }

        public IList<KeyValue> StoreList { get; set; }
        public IList<KeyValue> PaymentTypeList { get; set; }

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

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandGetRmaByOrder { get; set; }
        public DelegateCommand CommandGetRmaSaleDetailByRma { get; set; }
        public DelegateCommand CommandCustomerReturnGoodsSave { get; set; }
        public DelegateCommand CommandGetRmaDetailByRma { get; set; }
        public DelegateCommand CommandSetRmaRemark { get; set; }

        public decimal RmaDecimal
        {
            get { return rmaDecimal; }
            set { SetProperty(ref rmaDecimal, value); }
        }

        public SaleRmaDto SaleRma
        {
            get { return saleRma; }
            set { SetProperty(ref saleRma, value); }
        }

        public void SearchSaleRma()
        {
            if (RmaDetailList != null)
            {
                RmaDetailList.Clear();
            }
            if (RmaList != null)
            {
                RmaList.Clear();
            }
            try
            {
                SaleRmaList =
                    AppEx.Container.GetInstance<IFinancialPayVerify>().GetRmaByReturnGoodPay(ReturnGoodsPayDto).ToList();
            }
            catch
            {

            }
        }

        public void GetRmaDetailByRma()
        {
            if (RmaDto == null)
            {
                //MessageBox.Show("请选择退货单", "提示");
                return;
            }
            RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(RmaDto.RMANo).ToList();
        }

        public void GetRmaByOrder()
        {
            if (SaleRma == null)
            {
                RmaList.Clear();
                RmaDetailList.Clear();
                return;
            }
            try
            {
                RmaList = AppEx.Container.GetInstance<IFinancialPayVerify>().GetRmaByRmaOder(SaleRma.RmaNo).ToList();
            }
            catch
            {
            }
        }

        public void CustomerReturnGoodsSave()
        {
            if (RmaDecimal == 0)
            {
                MessageBox.Show("实退总金额必须大于0", "提示");
                return;
            }
            if (SaleRma == null)
            {
                MessageBox.Show("请选择收货单", "提示");
                return;
            }
            bool falg = AppEx.Container.GetInstance<IFinancialPayVerify>()
                .ReturnGoodsPayVerify(SaleRma.RmaNo, rmaDecimal);
            MessageBox.Show(falg ? "退货付款确认成功" : "退货付款确认失败", "提示");
            if (falg)
            {
                SearchSaleRma();
            }
        }

        public void InitCombo()
        {
            // OderStatusList=new 
            StoreList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();
            PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
        }

        public void GetRmaSaleDetailByRma()
        {
            if (rmaDto != null)
            {
                RmaDetailList = AppEx.Container.GetInstance<IPackageService>().GetRmaDetailByRma(rmaDto.RMANo).ToList();
            }
        }

        public void SetRmaRemark()
        {
            string id = RmaDto.RMANo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetRMARemark);
        }
    }
}