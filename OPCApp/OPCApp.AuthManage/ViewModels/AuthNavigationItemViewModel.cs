using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
namespace OPCApp.AuthManage.ViewModels
{
 
       [Export(typeof(AuthNavaeigationItemViewModel))]
       public class AuthNavaeigationItemViewModel
       {
           /// <summary>
           /// Initializes a new instance of the <see cref="MenuViewModel"/> class.
           /// </summary>
           /// <param name="menuService">The menu service.</param>
           [ImportingConstructor]
           public AuthNavaeigationItemViewModel(IMenuDataService menuService)
           {
               this.GroupItems = menuService.GetMenus();

           }
           public DelegateCommand<string> MenuClickCommand { get; set; }
           public DelegateCommand ClickCommand { get; set; }
           /// <summary>
           /// Gets or sets the items.
           /// </summary>
           /// <value>The items.</value>
           public IEnumerable<MenuGroup> GroupItems { get; set; }
       }

   
}

