using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.ReturnGoodsManage.Common;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
     [Export("ReturnAcceptCashierViewModel", typeof(ReturnAcceptCashierViewModel))]
    public class ReturnAcceptCashierViewModel : BaseReturnGoodsSearchCommonWithRma
    {
         /// <summary>
         /// 退货入收银
         /// </summary>
        public DelegateCommand CommandReturnAcceptCashier { get; set; }

         /// <summary>
         /// 完成退货入收银
         /// </summary>
        public DelegateCommand CommandReturnAcceptCashierConfirm { get; set; }
        public ReturnAcceptCashierViewModel()
        {
          
            CommandSearch = new DelegateCommand(SearchRma);
            CommandReturnAcceptCashier = new DelegateCommand(ReturnAcceptCashier);
            CommandReturnAcceptCashierConfirm = new DelegateCommand(ReturnAcceptCashierConfirm);
        }

        private void ReturnAcceptCashierConfirm()
        {
            throw new System.NotImplementedException();
        }

        private void ReturnAcceptCashier()
        {
            throw new System.NotImplementedException();
        }

        public override void SearchRma()
        {
            CustomReturnGoodsUserControlViewModel.RmaList = null;
        }
    }
}
