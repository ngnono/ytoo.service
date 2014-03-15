using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels.Role
{
    [Export("RoleViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleViewModel : BaseViewModel<Domain.Role>
    {
        public RoleViewModel()
            : base("RoleAddView")
        {
            this.Model = new Domain.Role();
        }
         /*选择字段*/
       public string SelectedFiled { get; set; }
       /*选择字段的值*/
       public string SelectedFiledValue { get; set; }
     
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
