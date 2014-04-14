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
        private IList<OPC_OrgInfo> _orgList;
        private OPC_OrgInfo orgInfo;

        public UserAddWindowViewModel()
            : base("UserView")
        {
            Model = new OPC_AuthUser();
            OrgList = AppEx.Container.GetInstance<IOrgService>().Search();
            OrgInfo = new OPC_OrgInfo();
        }

        public OPC_OrgInfo OrgInfo
        {
            get { return orgInfo; }
            set { SetProperty(ref orgInfo, value); }
        }

        public IList<OPC_OrgInfo> OrgList
        {
            get { return _orgList; }
            set { SetProperty(ref _orgList, value); }
        }

        public override bool BeforeDoOKAction(OPC_AuthUser t)
        {
            if (OrgInfo == null)
            {
                return true;
            }
            t.DataAuthId = OrgInfo.OrgID;
            t.DataAuthName = OrgInfo.OrgName;
            return true;
        }

        protected override IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }
}