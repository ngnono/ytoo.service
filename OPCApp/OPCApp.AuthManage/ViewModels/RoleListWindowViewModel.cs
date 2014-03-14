using OPCApp.AuthManage.Views;
using OPCApp.Domain;
using OPCApp.DataService.Impl;
namespace OPCApp.AuthManage.ViewModels
{
    public class RoleListWindowViewModel : OPCApp.Infrastructure.Mvvm.ViewModel4Grid<Role>
    {
        public RoleListWindowViewModel():base()
        {

        }


        protected override Role DoAddAction()
        {
            throw new System.NotImplementedException();
        }

        protected override void DoEditAction(Role t)
        {
            throw new System.NotImplementedException();
        }

        protected override Main.Infrastructure.DataService.IBaseDataService<Role> GetDataService()
        {
            return new RoleDataService();
        }

        protected override Infrastructure.Mvvm.IViewModel<Role> GetViewModel()
        {
            return new RoleViewModel(new RoleAddWindow());
        }
    }

}
