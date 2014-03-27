using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels
{
    [Export("UserViewModel", typeof (IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAddWindowViewModel : BaseViewModel<OPC_AuthUser>
    {
        public UserAddWindowViewModel()
            : base("UserView")
        {
            Model = new OPC_AuthUser();
        }

        protected override IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }
}