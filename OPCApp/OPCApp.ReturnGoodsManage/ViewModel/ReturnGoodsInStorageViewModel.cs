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
      [Export(typeof(ReturnGoodsInStorageViewModel))]
    public class ReturnGoodsInStorageViewModel : BaseReturnGoodsSearchCommonWithRma
    {        
         /// <summary>
         /// 退货入库
         /// </summary>
        public DelegateCommand CommandReturnGoodsConfirm { get; set; }
        public ReturnGoodsInStorageViewModel()
        {
            CommandReturnGoodsConfirm = new DelegateCommand(ReturnGoodsInStorage);
        }

        private void ReturnGoodsInStorage()
        {
            if (VerifyRmaSelected())
            {
                var rmaList = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsInStorage(rmaList);
            }
        }

        public override void SearchRma()
        {
            CustomReturnGoodsUserControlViewModel.RmaList = null;
        }
    }
}
