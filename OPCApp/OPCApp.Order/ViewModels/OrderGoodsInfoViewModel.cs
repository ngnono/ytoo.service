using System.Collections.Generic;
using OPCApp.Order.Model;

namespace OPCApp.Order.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderGoodsInfoViewModel : ViewModelBase
    {
        public OrderInfo OrderInfo { get; set; }
        public IList<OrderGoodsInfo> GoodsInfos { get; set; }
    }
}