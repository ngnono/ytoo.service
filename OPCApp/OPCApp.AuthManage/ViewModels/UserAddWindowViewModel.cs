using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Domain.Models;
namespace OPCApp.AuthManage.ViewModels
{
    [Export("UserViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAddWindowViewModel : BaseViewModel<OPC_AuthUser>
    {
        public UserAddWindowViewModel()
            : base("UserView")
        {
            this.Model = new OPC_AuthUser();
        }

        protected override Infrastructure.DataService.IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }
   
}

