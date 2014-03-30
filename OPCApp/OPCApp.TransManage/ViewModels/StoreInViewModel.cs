using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("StoreInViewModel", typeof (StoreInViewModel))]
    public class StoreInViewModel : PrintInvoiceViewModel
    {
        public StoreInViewModel()
        {
            SearchSaleStatus = EnumSearchSaleStatus.StoreInDataBaseSearchStatus;
            //初始化命令属性
            CommandSoldOut = new DelegateCommand(CommandSoldOutExecute);
            CommandStoreInSure = new DelegateCommand(CommandStoreInSureExecute);
        }

        public DelegateCommand CommandSoldOut { get; set; }
        public DelegateCommand CommandStoreInSure { get; set; }


        public void CommandSoldOutExecute()
        {
            if (SaleList == null||!SaleList.Any())
            {
                MessageBox.Show("请勾选要设置缺货状态的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusSoldOut(selectSaleIds);
            MessageBox.Show(bFalg ? "设置缺货成功" : "设置缺货失败", "提示");
            Refresh();
        }

        public void CommandStoreInSureExecute()
        {
            if (SaleList == null||!SaleList.Any())
            {
                MessageBox.Show("请勾选要设置入库的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusStoreInSure(selectSaleIds);
            MessageBox.Show(bFalg ? "销售单入库成功" : "销售单入库失败", "提示");
            Refresh();
        }
    }
}