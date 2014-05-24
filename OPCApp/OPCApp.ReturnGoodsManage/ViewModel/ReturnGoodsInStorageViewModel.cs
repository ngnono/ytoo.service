using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;
using System.Windows.Forms;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ReturnGoodsInStorageViewModel))]
    public class ReturnGoodsInStorageViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public ReturnGoodsInStorageViewModel()
        {
            CommandReturnGoodsConfirm = new DelegateCommand(ReturnGoodsInStorage);
        }

        /// <summary>
        ///     退货入库
        /// </summary>
        public DelegateCommand CommandReturnGoodsConfirm { get; set; }

        private void ReturnGoodsInStorage()
        {
            if (VerifyRmaSelected())
            {
                List<string> rmaList = GetRmoNoList();
                bool falg = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().SetReturnGoodsInStorage(rmaList);
                MessageBox.Show(falg ? "物流入库成功" : "物流入库失败", "提示");
                if (falg)
                {
                    CustomReturnGoodsUserControlViewModel.RmaList.Clear();
                    CustomReturnGoodsUserControlViewModel.RmaDetailList.Clear();
                    SearchRma();
                }
            }
        }

        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForReturnInStorage(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}