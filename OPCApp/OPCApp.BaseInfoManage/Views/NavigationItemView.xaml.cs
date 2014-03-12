//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;//.Regions;
using OPCApp.Infrastructure;
using OPCApp.BaseInfoManage.ViewModels;
using OPCApp.Main.Infrastructure;//.Infrastructure;


namespace OPCApp.BaseInfoManage.Views
{
    [Export]
    public partial class NavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri StoreManageUrl = new Uri("StoreManage", UriKind.Relative);
        private static Uri roleListViewUri = new Uri("RoleListWindow", UriKind.Relative);
        [Import]
        public IRegionManager regionManager;

        public NavigationItemViewModel aniv = new NavigationItemViewModel();
        public NavigationItemView()
        {
            InitializeComponent();
            aniv.UserListCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(new Action(StoreManageCommond));
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

        private void StoreManageCommond()
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, StoreManageUrl);
        }
        private void RoleListCommand()
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, roleListViewUri);
        }
    }
}
