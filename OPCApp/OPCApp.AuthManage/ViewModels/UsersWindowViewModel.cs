
﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
﻿using System.Linq;
﻿using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using Microsoft.Practices.Prism.Mvvm;
namespace OPCApp.AuthManage.ViewModels
{
    [Export("UsersViewModel", typeof(UsersWindowViewModel))]
  
    public class UsersWindowViewModel : BindableBase
    {
       /*选择字段*/
       public string SelectedFiled { get; set; }
       /*选择字段的值*/
       public string SelectedFiledValue { get; set; }
       /*查询字段列表*/
       public List<string> FieldList { get; set; }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand CommandGetDown { get; set; }

        public UsersWindowViewModel()
        {
            this.Init();
        }

        private void Search()
        {

            var dicFilter = new Dictionary<string, object> { { this.FieldList.IndexOf(SelectedFiled).ToString(), SelectedFiledValue } };
            var  userDataService = this.GetDataService();
            userDataService.Search(dicFilter);
        }
        /*初始化页面固有的数据值*/
        private void Init() 
        {
            this.FieldList = new List<string> {"登陆名", "专柜码", "姓名", "门店", "机构"};
            /*查询初始化*/
            this.SelectedFiledValue = "";
            this.SelectedFiled = "";
            this.SearchCommand = new DelegateCommand(this.Search);
            this.CommandGetDown = new DelegateCommand(this.SelectedUser);

        }
        private List<OPC_AuthUser> SelectedUserList { get; set; }
        private void SelectedUser()
        {
            this.SelectedUserList = UserList.Where(e => e.IsSelected).ToList();
        }

        private List<OPC_AuthUser> _userList;
        public List<OPC_AuthUser> UserList
        {

            get { return this._userList; }
            set { SetProperty(ref this._userList, value); }
        }
        protected  Infrastructure.DataService.IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }

}

