using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.AuthManage.ViewModels
{
    [Export("UsersViewModel", typeof (UsersWindowViewModel))]
    public class UsersWindowViewModel : BindableBase
    {
        /*选择字段*/
        private List<OPC_AuthUser> _userList;

        public UsersWindowViewModel()
        {
            Init();
        }

        public string SelectedFiled { get; set; }
        /*选择字段的值*/
        public string SelectedFiledValue { get; set; }
        /*查询字段列表*/
        public List<string> FieldList { get; set; }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        private List<OPC_AuthUser> SelectedUserList { get; set; }

        public List<OPC_AuthUser> UserList
        {
            get { return _userList; }
            set { SetProperty(ref _userList, value); }
        }

        private void Search()
        {
            var dicFilter = new Dictionary<string, object>
            {
                {FieldList.IndexOf(SelectedFiled).ToString(), SelectedFiledValue}
            };
            IBaseDataService<OPC_AuthUser> userDataService = GetDataService();
            userDataService.Search(dicFilter);
        }

        /*初始化页面固有的数据值*/

        private void Init()
        {
            FieldList = new List<string> {"登陆名", "专柜码", "姓名", "门店", "机构"};
            /*查询初始化*/
            SelectedFiledValue = "";
            SelectedFiled = "";
            SearchCommand = new DelegateCommand(Search);
            CommandGetDown = new DelegateCommand(SelectedUser);
        }

        private void SelectedUser()
        {
            SelectedUserList = UserList.Where(e => e.IsSelected).ToList();
        }

        protected IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }
}