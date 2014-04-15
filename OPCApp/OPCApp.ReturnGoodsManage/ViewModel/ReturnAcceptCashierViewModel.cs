using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ReturnAcceptCashierViewModel))]
    public class ReturnAcceptCashierViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public ReturnAcceptCashierViewModel()
        {
            CommandReturnAcceptCashier = new DelegateCommand(ReturnAcceptCashier);
            CommandReturnAcceptCashierConfirm = new DelegateCommand(ReturnAcceptCashierConfirm);
        }

        /// <summary>
        ///     退货入收银
        /// </summary>
        public DelegateCommand CommandReturnAcceptCashier { get; set; }

        /// <summary>
        ///     完成退货入收银
        /// </summary>
        public DelegateCommand CommandReturnAcceptCashierConfirm { get; set; }

        private void ReturnAcceptCashierConfirm()
        {
            if (VerifyRmaSelected())
            {
                List<string> listRmaNo = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsCash(listRmaNo);
                MessageBox.Show(falg ? "退货入收银成功" : "退货入收银失败", "提示");
                Refresh();
            }
        }

        private void ReturnAcceptCashier()
        {
            if (VerifyRmaSelected())
            {
                List<string> listRmaNo = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsComplete(listRmaNo);
                MessageBox.Show(falg ? "完成退货入收银成功" : "完成退货入收银失败", "提示");
                Refresh();
            }
        }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForReturnCash(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}