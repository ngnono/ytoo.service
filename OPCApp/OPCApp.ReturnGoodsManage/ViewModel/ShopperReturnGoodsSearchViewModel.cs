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
    [Export(typeof(ShopperReturnGoodsSearchViewModel))]
    public class ShopperReturnGoodsSearchViewModel : BaseReturnGoodsSearchCommonWithRma
    {        
        public ShopperReturnGoodsSearchViewModel()
        {          
            
        }
        public override void SearchRma()
        {
            try
            {
                CustomReturnGoodsUserControlViewModel.RmaList = AppEx.Container.GetInstance<IReturnGoodsSearchWithRma>().GetRmaForShopperReturnOrReceivingPrintDoc(this.ReturnGoodsCommonSearchDto).ToList();

            }
            catch
            {

            }
        }   
    }
}
