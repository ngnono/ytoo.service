using System.ComponentModel.Composition;
using OPCApp.AuthManage.Views;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.DataService.Impl;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;
namespace OPCApp.AuthManage.ViewModels
{

    [Export("RoleListViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleListViewModel :BaseCollectionViewModel<Role>
    {
        public RoleListViewModel()
            : base("RoleListWindow")
        {
            this.EditViewModeKey = "RoleViewModel";
            this.AddViewModeKey = "RoleViewModel";
        }
       
        protected override IBaseDataService<Role> GetDataService()
        {
            return AppEx.Container.GetInstance<IRoleDataService>();
        }
       
    }

}
