using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Dto;
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
            OrgTypeList = new List<KeyValue>() { new KeyValue(0, "组织机构"), new KeyValue(5, "门店"), new KeyValue(10, "专柜") };
            OrgTypeChangeCommand = new DelegateCommand<int?>(OrgTypeChange);
           
        }

        public void OrgTypeChange(int? orgType)
        {
           GetOrgRefreshStoreOrSection(orgType);
        }

        public void GetOrgRefreshStoreOrSection(int? orgType)
        {
            switch (orgType)
            {
                case 5:
                    StoreOrSectionList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();
                    break;
                case 10:
                    StoreOrSectionList = AppEx.Container.GetInstance<ICommonInfo>().GetSectionList();
                    break;
                default:
                    StoreOrSectionList = new List<KeyValue>();
                    break;
            }

        }

        public DelegateCommand<int?> OrgTypeChangeCommand { get; set; }
        public List<KeyValue> OrgTypeList { get; set; }
        private IList<KeyValue> _storeOrSectionList;
        public IList<KeyValue> StoreOrSectionList
        {
            get { return _storeOrSectionList; }
            set { SetProperty(ref _storeOrSectionList, value); }
        }


        protected override IBaseDataService<OPC_OrgInfo> GetDataService()
        {
            return AppEx.Container.GetInstance<IOrgService>();
        }
    }
}