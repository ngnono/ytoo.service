using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using MahApps.Metro;
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
        public NodeViewModel Nodes { get; private set; }
        public NodeInfo NodeInfo { get; private set; }
        public ReadOnlyCollection<DelegateCommand> Commands { get; private set; }

        private void InitOrg()
        {
            var orgList = AppEx.Container.GetInstance<IOrgService>().Search().ToList();
            Commands = new ReadOnlyCollection<DelegateCommand>(new DelegateCommand[]
            {
                new DelegateCommand(SearchAction)
            });

            NodeInfo = new NodeInfo();
            NodeInfo.SelectedNodeChanged += (s, e) => RefreshCommands();
            Nodes = new NodeViewModel(NodeInfo);
            GetNodesTree(Nodes,orgList);
        }

        public void GetNodesTree(NodeViewModel node,List<OPC_OrgInfo> listOrg )
        {
            var orgParent = listOrg.Where(e => e.OrgID == e.ParentID).ToList();
            foreach (var opcOrgInfo in orgParent)
            {
                var nv = node.AddSubNode(opcOrgInfo);
                GetNodesTreeChild(nv, listOrg);   
            }
         
        }
        public void GetNodesTreeChild(NodeViewModel node, List<OPC_OrgInfo> listOrg)
        {
            var orgParent = listOrg.Where(e => e.OrgID != e.ParentID&&e.ParentID==node.OrgId).ToList();
            foreach (var opcOrgInfo in orgParent)
            {
                var nv = node.AddSubNode(opcOrgInfo);
                GetNodesTreeChild(nv, listOrg);   
            }

        }
        private bool CheckSelection()
        {
            return NodeInfo.SelectedNode != null;
        }
        void RefreshCommands()
        {
            foreach (var cmd in Commands)
            {
                cmd.Execute();
                //if (cmd.IsActive)
                //    cmd.RaiseCanExecuteChanged();
            }
        }
     
        NodeViewModel GetOperationNode()
        {
            if (NodeInfo.SelectedNode == null)
                return Nodes;
            return NodeInfo.SelectedNode;
        }
       











        public PageResult _prResult;
        public PageResult PrResult
        {
            get { return _prResult; }
            set { SetProperty(ref _prResult, value); }
        }
        /*选择字段*/

        public UserListWindowViewModel()
            : base("UserListWindow")
        {
            EditViewModeKey = "UserViewModel";
            AddViewModeKey = "UserViewModel";
            InitOrg();
            InitUser();
        }

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
        public DelegateCommand DBGridClickCommand { get; set; }

        protected override IDictionary<string, object> GetFilter()
        {
            var node = GetOperationNode();
            var indexFiled = FieldList.IndexOf(SelectedFiled);
            var dicFilter = new Dictionary<string, object>
            {
                {"SearchField",indexFiled ==-1?1:indexFiled},
                {"SearchValue", SelectedFiledValue},
                 {"pageIndex", this.PageIndex},
                  {"pageSize", this.PageSize},
                  {"orgid",node.OrgId}
            };
            return dicFilter;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public override void SearchAction()
        {
            var  PrResultTemp = AppEx.Container.GetInstance<IAuthenticateService>().Search(GetFilter());
            if (PrResultTemp == null||PrResultTemp.Result==null)
            {
                return;
            }
            PrResult=new PageResult();
            PrResult.Models = PrResultTemp.Result.ToList();
            PrResult.Total = PrResultTemp.TotalCount;
        }

        private void DBGridClick()
        {
        }

        /*初始化页面固有的数据值*/

        private void InitUser()
        {
            FieldList = new List<string> {"登陆名","姓名"};
            /*查询初始化*/
            SelectedFiledValue = "";
            SelectedFiled = "";
            SetStopUserCommand = new DelegateCommand(SetStopUser);
            DBGridClickCommand = new DelegateCommand(DBGridClick);
            PageIndex = 1;
            PageSize = 10;
            PrResult=new PageResult();
        }

        private void SetStopUser()
        {
            var user = Model as OPC_AuthUser;
            if (user != null)
            {
                var iauth = GetDataService() as IAuthenticateService;
                bool isValid = user.IsValid == true ? true : false;
                iauth.SetIsStop(user.Id, isValid);

                //SetIsStop(user.Id, isValid);
            }
        }

        protected override IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }

        public class PageResult :BindableBase
        {

            public int _total;
            public int Total
            {
                get { return _total; }
                set { SetProperty(ref _total, value); }
            }

            private List<OPC_AuthUser> _models;
            public List<OPC_AuthUser> Models
            {
                get { return _models; }
                set { SetProperty(ref _models, value); }
            }

            public PageResult()
            {
                Models = new List<OPC_AuthUser>();
            }

        }
    }
}