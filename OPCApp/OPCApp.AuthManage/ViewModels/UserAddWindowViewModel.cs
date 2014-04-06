using System.Collections.Generic;
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
            OrgList = AppEx.Container.GetInstance<IOrgService>().Search();
            OrgInfo = new OPC_OrgInfo();
        }

        private OPC_OrgInfo orgInfo;
        public OPC_OrgInfo OrgInfo
        {
            get { return orgInfo; }
            set { SetProperty(ref orgInfo, value); }
        }
        public override bool BeforeDoOKAction(OPC_AuthUser t)
        {
            //t.DataAuthId = orgInfo.OrgID;
            //t.DataAuthName = orgInfo.OrgName;
            return true;
        } 
        private IList<OPC_OrgInfo> _orgList;
        public IList<OPC_OrgInfo> OrgList
        {
            get { return _orgList; }
            set { SetProperty(ref _orgList, value); }
        }
        protected override IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }
}