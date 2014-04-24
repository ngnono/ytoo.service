using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.Order.Model;

namespace OPCApp.Order.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderInfoSearchViewModel : ViewModelBase
    {
        public OrderInfoSearchViewModel()
        {
            ReturnOrders = new List<OrderInfo>();
        }

        public IList<OrderInfo> ReturnOrders { get; set; }
    }
}