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
    [Export("CustomerReturnSearchViewModel", typeof (CustomerReturnSearchViewModel))]
    public class CustomerReturnSearchViewModel : BindableBase
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
        public ReturnGoodsInfoGet ReturnGoodsInfoGet { get; set; }

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
            List<RMADto> rmaSelectedList = RMADtoList.Where(e => e.IsSelected).ToList();
            if (rmaSelectedList.Count == 0)
            {
                MessageBox.Show("请选择退货单", "提示");
                return;
            }
            bool falg =
                AppEx.Container.GetInstance<ICustomerReturnSearch>()
                    .AgreeReturnGoods(rmaSelectedList.Select(e => e.RMANo).ToList());
            MessageBox.Show(falg ? "退货成功" : "退货失败", "提示");
            if (falg)
            {
                RmaDetailList.Clear();
                RMADtoList.Clear();
            }
        }

        public void SearchGoodsInfo()
        {
            OrderDtoList =
                AppEx.Container.GetInstance<ICustomerReturnSearch>().ReturnGoodsSearch(ReturnGoodsInfoGet).ToList();
        }

        public void SearchRmaDtoListInfo()
        {
            if (OrderDto == null)
            {
                RmaDetailList.Clear();
                return;
            }
            RMADtoList = AppEx.Container.GetInstance<ICustomerReturnSearch>().GetRmaByOrderNo(OrderDto.OrderNo).ToList();
        }
    }
}