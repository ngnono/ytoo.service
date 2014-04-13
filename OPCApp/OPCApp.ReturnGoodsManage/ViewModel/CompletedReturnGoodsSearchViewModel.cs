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
     [Export(typeof(CompletedReturnGoodsSearchViewModel))]
    public class CompletedReturnGoodsSearchViewModel : BaseReturnGoodsSearchCommonWithRma
    {  
        public override void SearchRma()
        {
            CustomReturnGoodsUserControlViewModel.RmaList = null;
        }
    }
      
}
