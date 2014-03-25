using System.ComponentModel.Composition;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.BaseInfoManage.ViewModels
{
    [Export("StoreViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreAddWindowViewModel : BaseViewModel<Store>
    {
        public StoreAddWindowViewModel()
            : base("StoreView")
        {
            this.Model = new Store();
        }

        protected override Infrastructure.DataService.IBaseDataService<Store> GetDataService()
        {
            return AppEx.Container.GetInstance<OPCApp.DataService.Interface.IStoreService>();
        }
    }
}
