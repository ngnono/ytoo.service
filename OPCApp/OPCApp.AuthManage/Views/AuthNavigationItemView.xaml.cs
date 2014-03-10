
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using OPCApp.AuthManage.ViewModels;
/*
 hanyuxing
 */

namespace OPCApp.AuthManage.Views
{
    [Export]
    public partial class AuthNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri userListViewUri = new Uri("UserListWindow", UriKind.Relative);
        private static Uri roleListViewUri = new Uri("RoleListWindow", UriKind.Relative);
        [Import]
        public IRegionManager regionManager;

        public AuthNavigationItemViewModel aniv = new AuthNavigationItemViewModel();
        public AuthNavigationItemView()
        {
            InitializeComponent();
            aniv.UserListCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(new Action(UserListCommond));
            aniv.RoleListCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(new Action(RoleListCommand));
            this.DataContext = aniv;
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            //this.NavigateToCalendarRadioButton.IsChecked = (uri == calendarViewUri);
        }

        private void UserListCommond()
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, userListViewUri);
        }
        private void RoleListCommand()
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, roleListViewUri);
        }
    }
}
