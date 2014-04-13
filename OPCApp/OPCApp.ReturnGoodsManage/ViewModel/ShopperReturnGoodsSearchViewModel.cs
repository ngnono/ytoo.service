using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.ReturnGoodsManage.ViewModel
{
     [Export(typeof(ShopperReturnGoodsSearchViewModel))]
    public class ShopperReturnGoodsSearchViewModel : BindableBase
    {
         private CustomReturnGoodsUserControlViewModel _curtomReturnGoodsUcViewModel;
        [Import(typeof(CustomReturnGoodsUserControlViewModel))]
         public CustomReturnGoodsUserControlViewModel CustomReturnGoodsUserControlViewModel
        {
            get { return _curtomReturnGoodsUcViewModel; }
            set{SetProperty(ref _curtomReturnGoodsUcViewModel,value);}
        }
        private PackageReceiveDto _packageReceiveDto;
        //与包裹审核公用传输类
        public PackageReceiveDto PackageReceiveDto
        {
            get { return _packageReceiveDto; }
            set { SetProperty(ref _packageReceiveDto, value); }
        }
        public DelegateCommand CommandSearch { get; set; }
        public ShopperReturnGoodsSearchViewModel()
        {
            PackageReceiveDto = new PackageReceiveDto();
            CommandSearch = new DelegateCommand(SearchRma);
        }

        private void SearchRma()
        {
            CustomReturnGoodsUserControlViewModel.RmaList = AppEx.Container.GetInstance<IPackageService>().GetRmaByFilter(PackageReceiveDto).ToList();
        }
     
    }
}
