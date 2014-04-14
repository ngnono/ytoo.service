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
    [Export(typeof (ReturnGoodsEntryPrintViewModel))]
    public class ReturnGoodsEntryPrintViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public ReturnGoodsEntryPrintViewModel()
        {
            CommandPrintPreview = new DelegateCommand(PrintPreView);
            CommandPrintReturnGoodsConfirm = new DelegateCommand(ReturnGoodsConfirm);
        }

        public DelegateCommand CommandPrintReturnGoodsConfirm { get; set; }

        public DelegateCommand CommandPrintPreview { get; set; }

        private void ReturnGoodsConfirm()
        {
            if (VerifyRmaSelected())
            {
                List<string> rmaNos = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoodsComplete(rmaNos);
                MessageBox.Show(falg ? "设置打印完成" : "设置打印完成失败", "提示");
                if (falg)
                {
                    Refresh();
                }
            }
        }

        private void PrintPreView()
        {
            if (VerifyRmaSelected())
            {
                List<string> rmaNos = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoods(rmaNos);
                MessageBox.Show(falg ? "打印退货单成功" : "打印退货单失败", "提示");
            }
        }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForReturnPrintDoc(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}