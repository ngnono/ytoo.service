using OPCApp.Order.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Order.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderGoodsInfoViewModel : ViewModelBase
    {
        public OrderInfo OrderInfo { get; set; }
        public IList<OrderGoodsInfo> GoodsInfos
        {
            get;
            set;
        }
    }
}
