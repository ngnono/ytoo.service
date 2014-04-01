using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserListWindowViewModel : BaseListViewModel<OPC_AuthUser>
    {
        public PageResult<OPC_AuthUser> PrResult1 { get; set; }
        public PageResult PrResult { get; set; }
        /*选择字段*/

        public UserListWindowViewModel()
            : base("UserListWindow")
        {
            EditViewModeKey = "UserViewModel";
            AddViewModeKey = "UserViewModel";
            Init();
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
            var dicFilter = new Dictionary<string, object>
            {
                {"SearchField", FieldList.IndexOf(SelectedFiled).ToString()},
                {"SearchValue", SelectedFiledValue}
            };
            return dicFilter;
        }

        private void DBGridClick()
        {
        }

        /*初始化页面固有的数据值*/

        private void Init()
        {
            FieldList = new List<string> {"登陆名", "专柜码", "姓名", "门店", "机构"};
            /*查询初始化*/
            SelectedFiledValue = "";
            SelectedFiled = "";
            SetStopUserCommand = new DelegateCommand(SetStopUser);
            DBGridClickCommand = new DelegateCommand(DBGridClick);
            PrResult1 = new PageResult<OPC_AuthUser>(null,2);
            PrResult1 = GetDataService().Search(null);
            PrResult = new PageResult();
            PrResult.Models = PrResult1.Result.ToList();
            PrResult.Total = PrResult1.TotalCount;
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

        public class PageResult : BindableBase
        {

            public int _total;
            public int Total
            {
                get { return _total; }
                set { SetProperty(ref _total, value); }
            }

            private List<OPC_AuthUser> _students;
            public List<OPC_AuthUser> Models
            {
                get { return _students; }
                set { SetProperty(ref _students, value); }
            }

            public PageResult()
            {
                Models = new List<OPC_AuthUser>();
            }

        }
    }
}