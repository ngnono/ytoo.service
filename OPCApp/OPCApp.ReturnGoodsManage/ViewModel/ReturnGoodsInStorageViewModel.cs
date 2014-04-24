using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;

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