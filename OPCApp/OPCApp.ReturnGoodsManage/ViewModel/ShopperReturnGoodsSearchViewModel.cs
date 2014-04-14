using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (ShopperReturnGoodsSearchViewModel))]
    public class ShopperReturnGoodsSearchViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForShopperReturnOrReceivingPrintDoc(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}