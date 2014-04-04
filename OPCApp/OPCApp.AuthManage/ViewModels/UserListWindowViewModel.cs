using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels
{
    [Export("UserListViewModel", typeof (IViewModel))]
    // [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserListWindowViewModel : BaseListViewModel<OPC_AuthUser>
    {
        private OPC_AuthUser _curUser;
        public PageResult _prResult;

        public UserListWindowViewModel()
            : base("UserListWindow")
        {
            EditViewModeKey = "UserViewModel";
            AddViewModeKey = "UserViewModel";
            InitOrg();
            InitUser();
        }

        public DelegateCommand AddOrgCommand { get; set; }
        public DelegateCommand EditOrgCommand { get; set; }
        public DelegateCommand DeleteOrgCommand { get; set; }
        public NodeViewModel Nodes { get; private set; }
        public NodeInfo NodeInfo { get; private set; }
        public ReadOnlyCollection<DelegateCommand> Commands { get; private set; }

        public PageResult PrResult
        {
            get { return _prResult; }
            set { SetProperty(ref _prResult, value); }
        }

        public OPC_AuthUser CurModel
        {
            get { return _curUser; }
            set { SetProperty(ref _curUser, value); }
        }

        /*选择字段*/

        public string SelectedFiled { get; set; }
        /*选择字段的值*/
        public string SelectedFiledValue { get; set; }
        /*查询字段列表*/
        public List<string> FieldList { get; set; }
        /*是否停用*/
        public DelegateCommand SetStopUserCommand { get; set; }
        /*导出用户*/
        public DelegateCommand ExportUserCommand { get; set; }
        /*双击用户列表*/
        public DelegateCommand AddUserCommand { get; set; }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public void AddOrg()
        {
            if (CheckSelection())
            {
                GetOperationNode().AddOrg();
            }
        }

        public void EditOrg()
        {
            if (CheckSelection())
            {
                GetOperationNode().UpdateOrg();
            }
        }

        public void DeleteOrg()
        {
            if (CheckSelection())
            {
                GetOperationNode().DeleteOrg();
            }
        }

        private void InitOrg()
        {
            AddOrgCommand = new DelegateCommand(AddOrg);
            EditOrgCommand = new DelegateCommand(EditOrg);
            DeleteOrgCommand = new DelegateCommand(DeleteOrg);
            List<OPC_OrgInfo> orgList = AppEx.Container.GetInstance<IOrgService>().Search().ToList();
            Commands = new ReadOnlyCollection<DelegateCommand>(new[]
            {
                new DelegateCommand(SearchAction)
            });

            NodeInfo = new NodeInfo();
            NodeInfo.SelectedNodeChanged += (s, e) => RefreshCommands();
            Nodes = new NodeViewModel(NodeInfo);
            GetNodesTree(Nodes, orgList);
        }

        private void GetNodesTree(NodeViewModel node, List<OPC_OrgInfo> listOrg)
        {
            List<OPC_OrgInfo> orgParent = listOrg.Where(e => e.OrgID == e.ParentID).ToList();
            foreach (OPC_OrgInfo opcOrgInfo in orgParent)
            {
                NodeViewModel nv = node.AddSubNode(opcOrgInfo);
                GetNodesTreeChild(nv, listOrg);
            }
        }

        private void GetNodesTreeChild(NodeViewModel node, List<OPC_OrgInfo> listOrg)
        {
            List<OPC_OrgInfo> orgParent = listOrg.Where(e => e.OrgID != e.ParentID && e.ParentID == node.OrgId).ToList();
            foreach (OPC_OrgInfo opcOrgInfo in orgParent)
            {
                NodeViewModel nv = node.AddSubNode(opcOrgInfo);
                GetNodesTreeChild(nv, listOrg);
            }
        }

        private bool CheckSelection()
        {
            if (NodeInfo.SelectedNode == null)
            {
                MessageBox.Show("请选择组织机构节点", "提示");
                return false;
                ;
            }
            return true;
        }

        private void RefreshCommands()
        {
            foreach (DelegateCommand cmd in Commands)
            {
                cmd.Execute();
            }
        }

        private NodeViewModel GetOperationNode()
        {
            if (NodeInfo.SelectedNode == null)
                return Nodes;
            return NodeInfo.SelectedNode;
        }

        protected override IDictionary<string, object> GetFilter()
        {
            NodeViewModel node = GetOperationNode();
            int indexFiled = FieldList.IndexOf(SelectedFiled);
            var dicFilter = new Dictionary<string, object>
            {
                {"SearchField", indexFiled == -1 ? 1 : indexFiled},
                {"SearchValue", SelectedFiledValue},
                {"pageIndex", PageIndex},
                {"pageSize", PageSize},
                {"orgid", node.OrgId}
            };
            return dicFilter;
        }

        public override void SearchAction()
        {
            PageResult<OPC_AuthUser> PrResultTemp =
                AppEx.Container.GetInstance<IAuthenticateService>().Search(GetFilter());
            if (PrResultTemp == null || PrResultTemp.Result == null)
            {
                return;
            }
            PrResult = new PageResult();
            PrResult.Models = PrResultTemp.Result.ToList();
            PrResult.Total = PrResultTemp.TotalCount;
        }
        /*重写 在选择组织结构 才能进行用户增加操作*/
         protected virtual bool BeforeAdd(OPC_AuthUser t)
        {
            var curNode = GetOperationNode();
            if (curNode==null||curNode.OrgId==null)
            {
                MessageBox.Show("请选择部门", "提示");
                return false;
            }
             t.OrgId = curNode.OrgId;
             return true;
        }

        /*初始化页面固有的数据值*/

        private void InitUser()
        {
            FieldList = new List<string> {"登陆名", "姓名"};
            /*查询初始化*/
            SelectedFiledValue = "";
            SelectedFiled = "";
            SetStopUserCommand = new DelegateCommand(SetStopUser);
            AddUserCommand = new DelegateCommand(AddUser);
            PageIndex = 1;
            PageSize = 10;
            PrResult = new PageResult();
            CurModel = new OPC_AuthUser();
        }
 

        private void SetStopUser()
        {
            OPC_AuthUser user = CurModel;
            if (user == null)
            {
                MessageBox.Show("请选择要操作的用户", "提示");
                return;
                ;
            }
            var iauth = GetDataService() as IAuthenticateService;
            bool isValid = user.IsValid == true ? true : false;
            iauth.SetIsStop(user.Id, isValid);
        }

        protected override IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }

        public class PageResult : BindableBase
        {
            private List<OPC_AuthUser> _models;
            public int _total;

            public PageResult()
            {
                Models = new List<OPC_AuthUser>();
            }

            public int Total
            {
                get { return _total; }
                set { SetProperty(ref _total, value); }
            }

            public List<OPC_AuthUser> Models
            {
                get { return _models; }
                set { SetProperty(ref _models, value); }
            }
        }
    }
}