using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Infrastructure;
using OPCApp.ReturnGoodsManage.Common;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
    [Export(typeof (CompletedReturnGoodsSearchViewModel))]
    public class CompletedReturnGoodsSearchViewModel : BaseReturnGoodsSearchCommonWithRma
    {
        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList =
                    AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>()
                        .GetRmaForCompletedReturnGoods(ReturnGoodsCommonSearchDto)
                        .ToList();
            }
            catch
            {
            }
        }
    }
}