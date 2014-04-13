using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof(ReturnGoodsEntryPrintViewModel))]
    public class ReturnGoodsEntryPrintViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public ReturnGoodsEntryPrintViewModel()
        {
            CommandPrintPreview = new DelegateCommand(PrintPreView);
            CommandPrintReturnGoodsConfirm = new DelegateCommand(ReturnGoodsConfirm);
        }

        private void ReturnGoodsConfirm()
        {
            if (VerifyRmaSelected())
            {
                var rmaNos = GetRmoNoList();
                var falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoodsComplete(rmaNos);
                MessageBox.Show(falg ? "设置打印完成" : "设置打印完成失败", "提示");
                if (falg)
                {
                    Refresh();
                }
            }
        }

        public DelegateCommand CommandPrintReturnGoodsConfirm { get; set; }

        private void PrintPreView()
        {
            if (VerifyRmaSelected())
            {
                var rmaNos = GetRmoNoList();
                var falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().PrintReturnGoods(rmaNos);
                MessageBox.Show(falg ? "打印退货单成功" : "打印退货单失败", "提示");
            }
        }

        public DelegateCommand CommandPrintPreview { get; set; }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().GetRmaForReturnPrintDoc(this.ReturnGoodsCommonSearchDto).ToList();

            }
            catch
            {

            }
        }     
    }
}
