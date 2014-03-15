using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Domain;
using OPCApp.AuthManage.Views;
namespace OPCApp.AuthManage.ViewModels
{
    [Export("RoleViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleViewModel : BaseViewModel<Role>
    {
        public RoleViewModel()
            : base("RoleAddView")
        {
            this.Model = new Role();
        }
         /*选择字段*/
       public string selectedFiled { get; set; }
       /*选择字段的值*/
       public string selectedFiledValue { get; set; }
     
        /*初始化页面固有的数据值*/
        private void Init() 
        {
            //this.FieldList = new List<string>();
            //this.FieldList.Add("登陆名");
            //this.FieldList.Add("专柜码");
            //this.FieldList.Add("姓名");
            //this.FieldList.Add("门店");
            //this.FieldList.Add("机构");
           
            ///*查询初始化*/
            //this.selectedFiledValue = "";
            //this.selectedFiled = "";
            ///*初始化结构 IOC*/
            //this.userService = new AuthenticateService();
        }
        /**/
      
      
       
    }
}
