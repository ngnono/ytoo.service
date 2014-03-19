
﻿using System;
﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
﻿using System.Runtime.InteropServices;
﻿using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.Views;
using OPCApp.Domain;
using OPCApp.DataService.Interface;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
namespace OPCApp.AuthManage.ViewModels
{
    [Export("UserListViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserListWindowViewModel : BaseListViewModel<OPC_AuthUser>
    {
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
        public DelegateCommand DBGridClickCommand { get; set; }
        public UserListWindowViewModel()
            : base("UserListWindow")
        {
            this.EditViewModeKey = "UserViewModel";
            this.AddViewModeKey = "UserViewModel";
            this.Init();
        }

        protected override IDictionary<string, object> GetFilter()
        {
            var dicFilter = new Dictionary<string, object> { { this.FieldList.IndexOf(SelectedFiled).ToString(), SelectedFiledValue } };
           return dicFilter;
        }

        private void DBGridClick() 
        {

        }
        /*初始化页面固有的数据值*/
        private void Init() 
        {
            this.FieldList = new List<string> {"登陆名", "专柜码", "姓名", "门店", "机构"};
            /*查询初始化*/
            this.SelectedFiledValue = "";
            this.SelectedFiled = "";
            this.SetStopUserCommand = new DelegateCommand(this.SetStopUser);
            
        }
        private void SetStopUser() 
        {
            var user = Model as OPC_AuthUser;
            if (user != null)
            {
                var iauth= this.GetDataService() as IAuthenticateService;
                bool isValid = user.IsValid == true ? true : false;
                iauth.SetIsStop(user.Id, isValid);

                //SetIsStop(user.Id, isValid);
            }
          
        }
        protected override Infrastructure.DataService.IBaseDataService<OPC_AuthUser> GetDataService()
        {
            return AppEx.Container.GetInstance<IAuthenticateService>();
        }
    }

}

