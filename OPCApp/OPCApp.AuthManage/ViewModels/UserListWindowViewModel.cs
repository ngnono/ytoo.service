using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.Views;
using OPCApp.Domain;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Impl;
namespace OPCApp.AuthManage.ViewModels
{
   public class UserListWindowViewModel : BindableBase
    {
       /*选择字段*/
       public string selectedFiled { get; set; }
       /*选择字段的值*/
       public string selectedFiledValue { get; set; }
       public IAuthenticateService userService { get; set; }
       /*查询字段列表*/
       public List<string> FieldList { get; set; }
       private User user;
       /*列表选择用户 并且 也用于新增修改时接受的用户*/
      
       public User User
        {
            get { return this.user; }
            set { SetProperty(ref this.user, value); }
        }
       private List<User> userList;
       /*用户列表*/
       public List<User> UserList
       {
           get { return this.userList; }
           set { SetProperty(ref this.userList, value); }
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
        public UserListWindowViewModel() 
        {
            this.SearchCommand = new DelegateCommand(this.searchCommand);
            this.AddCommand = new DelegateCommand(this.addCommand);
            this.UpdateCommand = new DelegateCommand(this.updateCommand);
            this.DelCommand = new DelegateCommand(this.delCommand);
            this.SetStopUserCommand = new DelegateCommand(this.setStopUserCommand);
            this.Init();
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
            this.selectedFiledValue = "";
            this.selectedFiled = "";
            /*初始化结构 IOC*/
            this.userService = new AuthenticateService();
        }
        /**/
        private void searchCommand() 
        {
            this.refreshList();
        }
        private void addCommand()
        {
            this.User = new User();
            UserAddWindow userWin = new UserAddWindow();
            userWin.userAddWin.User =this.User;
            if (userWin.ShowDialog() == true)
            {
                this.userService.AddUser(User);
                this.refreshList();
            }
        }
        private void updateCommand()
        {
            UserAddWindow userWin = new UserAddWindow();
            userWin.userAddWin.User = this.User;
            if (userWin.ShowDialog() == true)
            {
                this.userService.UpdateUser(User);
                this.refreshList();
            }
        }
        private void delCommand()
        {
             this.userService.DelUser(User);
            this.refreshList();
        }
        private void refreshList() 
        {
            this.UserList = this.userService.GetUserList(this.selectedFiled,this.selectedFiledValue);
        }

        private void setStopUserCommand() 
        {
            bool isValid = user.IsValid ? true : false;
            this.userService.SetIsStop(isValid);
        }
       
      
    }

}
