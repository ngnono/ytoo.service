using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels
{
    [Export("OrgViewModel", typeof (IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrgAddWindowViewMode : BaseViewModel<OPC_OrgInfo>
    {
        public OrgAddWindowViewMode()
            : base("OrgView")
        {
            Model = new OPC_OrgInfo();
        }

        protected override IBaseDataService<OPC_OrgInfo> GetDataService()
        {
            return AppEx.Container.GetInstance<IOrgService>();
        }
    }
}