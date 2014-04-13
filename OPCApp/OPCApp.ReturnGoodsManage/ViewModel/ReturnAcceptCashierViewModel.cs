using System;
using System.Windows;
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
     [Export(typeof(ReturnAcceptCashierViewModel))]
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

         public ReturnAcceptCashierViewModel() : base()
        {
            CommandReturnAcceptCashier = new DelegateCommand(ReturnAcceptCashier);
            CommandReturnAcceptCashierConfirm = new DelegateCommand(ReturnAcceptCashierConfirm);
        }

        private void ReturnAcceptCashierConfirm()
        {
            if (VerifyRmaSelected())
            {
                var listRmaNo = GetRmoNoList();
                var falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsComplete(listRmaNo);
                MessageBox.Show(falg?"退货入收银成功":"退货入收银失败", "提示");
                Refresh();
            }
        }

        private void ReturnAcceptCashier()
        {
            if (VerifyRmaSelected())
            {
                var listRmaNo = GetRmoNoList();
                var falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsComplete(listRmaNo);
                MessageBox.Show(falg ? "完成退货入收银成功" : "完成退货入收银失败", "提示");
                Refresh();
            }
        }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().GetRmaForReturnCash(this.ReturnGoodsCommonSearchDto).ToList();

            }
            catch 
            {
                
            }
        }
    }
}
