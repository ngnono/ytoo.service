using System.ComponentModel.Composition;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Interface;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.BaseInfoManage.ViewModels
{
    [Export("StoreViewModel", typeof (IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreAddWindowViewModel : BaseViewModel<Store>
    {
        public StoreAddWindowViewModel()
            : base("StoreView")
        {
            Model = new Store();
        }

        protected override IBaseDataService<Store> GetDataService()
        {
            return AppEx.Container.GetInstance<IStoreDataService>();
        }
    }
}