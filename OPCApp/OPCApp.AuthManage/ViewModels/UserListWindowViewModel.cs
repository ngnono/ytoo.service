
﻿using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.Views;
using OPCApp.Domain;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Impl;
using System.ComponentModel.Composition;
namespace OPCApp.AuthManage.ViewModels
{
   public class UserListWindowViewModel : BindableBase
    {
       /*选择字段*/
       public string SelectedFiled { get; set; }
       /*选择字段的值*/
       public string SelectedFiledValue { get; set; }
       [Import]
       public IAuthenticateService UserService { get; set; }
       /*查询字段列表*/
       public List<string> FieldList { get; set; }
       private User _user;
       /*列表选择用户 并且 也用于新增修改时接受的用户*/
      
       public User User
        {
            get { return this._user; }
            set { SetProperty(ref this._user, value); }
        }
       private List<User> _userList;
       /*用户列表*/
       public List<User> UserList
       {
           get { return this._userList; }
           set { SetProperty(ref this._userList, value); }
       }
       /*查询用户命令*/
        public DelegateCommand SearchCommand { get; set; }
        /*增加用户命令*/
        public DelegateCommand AddCommand { get; set; }
        /*修改用户命令*/
        public DelegateCommand UpdateCommand { get; set; }
        /*删除用户命令*/
        public DelegateCommand DelCommand { get; set; }
        /*是否启用用户*/
        public DelegateCommand SetStopUserCommand { get; set; }
        /*导出用户*/
        public DelegateCommand ExportUserCommand { get; set; }
       /*是否停用*/
        /*构造*/
        public DelegateCommand DBGridClickCommand { get; set; }
        public UserListWindowViewModel() 
        {
            this.SearchCommand = new DelegateCommand(this.Search);
            this.AddCommand = new DelegateCommand(this.Add);
            this.UpdateCommand = new DelegateCommand(this.Update);
            this.DelCommand = new DelegateCommand(this.Delete);
            this.SetStopUserCommand = new DelegateCommand(this.SetStopUser);
            this.DBGridClickCommand = new DelegateCommand(this.DBGridClick);
            this.Init();
        }
        private void DBGridClick() 
        {

        }
        /*初始化页面固有的数据值*/
        private void Init() 
        {
            this.FieldList = new List<string>();
            this.FieldList.Add("登陆名");
            this.FieldList.Add("专柜码");
            this.FieldList.Add("姓名");
            this.FieldList.Add("门店");
            this.FieldList.Add("机构");
           
            /*查询初始化*/
            this.SelectedFiledValue = "";
            this.SelectedFiled = "";
            //this.userService = new AuthenticateService();
        }
        /**/
        private void Search() 
        {
            this.RefreshList();
        }
        private void Add()
        {
            this.User = new User();
            var  userWin = new UserAddWindow();
            userWin.UserAddWin.User =this.User;
            if (userWin.ShowDialog() == true)
            {
                this.UserService.AddUser(User);
                this.RefreshList();
            }
        }
        private void Update()
        {
            UserAddWindow userWin = new UserAddWindow();
            userWin.UserAddWin.User = this.User;
            if (userWin.ShowDialog() == true)
            {
                this.UserService.UpdateUser(User);
                this.RefreshList();
            }
        }
        private void Delete()
        {
            this.UserService.DelUser(User);
            this.RefreshList();
        }
        private void RefreshList() 
        {
            this.UserList = this.UserService.GetUserList(this.SelectedFiled,this.SelectedFiledValue);
        }

        private void SetStopUser() 
        {
            bool isValid = User.IsValid ? true : false;
            this.UserService.SetIsStop(isValid);
        }
       
      
    }

}

