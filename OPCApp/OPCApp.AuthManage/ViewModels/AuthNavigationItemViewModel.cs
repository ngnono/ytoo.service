using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface;
using OPCApp.Domain;

namespace OPCApp.AuthManage.ViewModels
{
    [Export(typeof (AuthNavaeigationItemViewModel))]
    public class AuthNavaeigationItemViewModel : BindableBase
    {
        private readonly IMenuDataService _menuDataService;

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        private IEnumerable<MenuGroup> menuGroups;

        /// <summary>
        ///     Initializes a new1 instance of the <see cref="MenuViewModel" /> class.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        [ImportingConstructor]
        public AuthNavaeigationItemViewModel(IMenuDataService menuService)
        {
            _menuDataService = menuService;

            //GroupItems = _menuDataService.GetMenus();
        }

        public DelegateCommand<string> MenuClickCommand { get; set; }
        public DelegateCommand ClickCommand { get; set; }

        public IEnumerable<MenuGroup> GroupItems
        {
            get { return menuGroups; }
            set { SetProperty(ref menuGroups, value); }
            //{
            //    return _menuDataService.GetMenus();
            //}
        }

        public void GetMenus()
        {
            GroupItems = _menuDataService.GetMenus();
        }
    }
}