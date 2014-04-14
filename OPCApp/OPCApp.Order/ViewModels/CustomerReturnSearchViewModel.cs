using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Customer;
using OPCApp.DataService.Interface.Customer;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Dto;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using System;

namespace OPCApp.Customer.ViewModels
{
    public class CustomerReturnSearchViewModel:BindableBase
    {
        private OrderDto _orderDto;
        private List<OrderDto> _orderDtoList;
        private List<RmaDetail> _rmaDetails;
        private RMADto _rmaDto;
        private List<RMADto> _rmaDtoList;

        public CustomerReturnSearchViewModel()
        {
            ReturnGoodsInfoGet = new ReturnGoodsInfoGet();
            CommandSearchGoodsInfo = new DelegateCommand(SearchGoodsInfo);
            CommandSearchRmaDtoInfo = new DelegateCommand(SearchRmaDtoListInfo);
            CommandAgreeReturnGoods = new DelegateCommand(SetAgreeReturnGoods);
            CommandGetRmaDetailByRmaNo = new DelegateCommand(GetRmaDetailByRmaNo);
            this.InitCombo();
        }
        public IList<KeyValue> StoreList { get; set; }
        public IList<KeyValue> GetReturnDocStatusList { get; set; }
        public IList<KeyValue> PaymentTypeList { get; set; }
        public void InitCombo()
        {
            // OderStatusList=new 
            StoreList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();
            PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
            GetReturnDocStatusList = AppEx.Container.GetInstance<ICommonInfo>().GetReturnDocStatusList();
        }
        public RMADto RMADto
        {
            get { return _rmaDto; }
            set { SetProperty(ref _rmaDto, value); }
        }

        public OrderDto OrderDto
        {
            get { return _orderDto; }
            set { SetProperty(ref _orderDto, value); }
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

        public List<OrderDto> OrderDtoList
        {
            get { return _orderDtoList; }
            set { SetProperty(ref _orderDtoList, value); }
        }

        public DelegateCommand CommandAgreeReturnGoods { get; set; }
        public DelegateCommand CommandSearchGoodsInfo { get; set; }
        public DelegateCommand CommandSearchRmaDtoInfo { get; set; }
        public DelegateCommand CommandGetRmaDetailByRmaNo { get; set; }
        private ReturnGoodsInfoGet returnGoodsInfo;
        public ReturnGoodsInfoGet ReturnGoodsInfoGet {
            get { return returnGoodsInfo; }
            set { SetProperty(ref returnGoodsInfo, value); }
        }

        public void GetRmaDetailByRmaNo()
        {
            if (RMADto != null)
            {
                RmaDetailList = AppEx.Container.GetInstance<ICustomerReturnSearch>()
                    .GetRmaDetailByRmaNo(RMADto.RMANo).ToList();
            }
        }

        public void SetAgreeReturnGoods()
        {
            if (RMADtoList == null) {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }

            List<RMADto> rmaSelectedList = RMADtoList.Where(e => e.IsSelected).ToList();          
            if (rmaSelectedList.Count == 0)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            bool flag = false;
            try
            {
                flag =
                    AppEx.Container.GetInstance<ICustomerReturnSearch>()
                        .AgreeReturnGoods(rmaSelectedList.Select(e => e.RMANo).ToList());
            }
            catch(Exception Ex)
            {

            }
            MessageBox.Show(flag ? "客服同意退货成功" : "客服同意退货失败", "提示");
            if (flag)
            {
                RmaDetailList.Clear();
                RMADtoList.Clear();
            }
        }

        public virtual void SearchGoodsInfo()
        {
            OrderDtoList =
                AppEx.Container.GetInstance<ICustomerReturnSearch>().ReturnGoodsRmaSearch(ReturnGoodsInfoGet).ToList();
        }

        public void SearchRmaDtoListInfo()
        {
            if (OrderDto == null)
            {
                if (RmaDetailList!=null) RmaDetailList.Clear();
                return;
            } 
            var rmaList= AppEx.Container.GetInstance<ICustomerReturnSearch>().GetRmaByOrderNo(OrderDto.OrderNo).ToList();
            RMADtoList = rmaList;
        }

    }
}