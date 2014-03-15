using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.Domain;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Domain.Models;
namespace OPCApp.AuthManage.ViewModels
{
    [Export("UserViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAddWindowViewModel: BaseViewModel<Role>
    {
        public UserAddWindowViewModel()
            : base("UserView")
        {
            this.Model = new OPC_AuthUser();
        }
    }
   
}

