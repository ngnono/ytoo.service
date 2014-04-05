using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Interfaces;

namespace OPCApp.AuthManage.ViewModels
{
    [Export(typeof (AuthNavaeigationItemViewModel))]
    public class AuthNavaeigationItemViewModel
    {
        private readonly IMenuDataService _menuDataService;

        /// <summary>
        ///     Initializes a new1 instance of the <see cref="MenuViewModel" /> class.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        [ImportingConstructor]
        public AuthNavaeigationItemViewModel(IMenuDataService menuService)
        {
            _menuDataService = menuService;
        }

        public DelegateCommand<string> MenuClickCommand { get; set; }
        public DelegateCommand ClickCommand { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<MenuGroup> GroupItems
        {
            get
            {
                AppEx.Container.GetInstance<ILoginManager>().Login("admin", "admin");
                // Logon(); 
                // var rest = RestClient.Post("Account/Token", new { UserName ="111", Password = "11" });
                return _menuDataService.GetMenus();
            }
        }
    }
}