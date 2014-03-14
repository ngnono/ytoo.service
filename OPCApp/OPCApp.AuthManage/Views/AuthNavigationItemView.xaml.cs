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
using OPCApp.AuthManage.ViewModels;//.Infrastructure;


namespace OPCApp.AuthManage.Views
{
    [Export]
    public partial class AuthNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        //private static Uri userListViewUri = new Uri("UserListWindow", UriKind.Relative);
        private static Uri userListViewUri = new Uri("MenuView", UriKind.Relative);
        private static Uri roleListViewUri = new Uri("RoleListWindow", UriKind.Relative);
        
        public MenuViewModel ViewMode
        {
            get
            {
                return this.DataContext as MenuViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    
        [Import]
        public IRegionManager regionManager;

        //public AuthNavigationItemViewModel aniv = new AuthNavigationItemViewModel();

        [ImportingConstructor]
        public AuthNavigationItemView(MenuViewModel viewModel)
        {
            InitializeComponent();
            ViewMode = viewModel;
            //this.yhid.Visibility = Visibility.Hidden;
            ViewMode.ClickCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(this.clickCommand);
            ViewMode.MenuClickCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand<string>(this.menuClickCommand);
            //this.DataContext = aniv;
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

        private void clickCommand()
        {
            MessageBox.Show("1");
        }
        private void menuClickCommand(string url)
        {
            Uri roleListViewUri = new Uri(url, UriKind.Relative);
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, roleListViewUri);
        }

      

        private void TextBlock_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var o = sender as TextBlock;
            var o1 = o.Parent as StackPanel;
            var o2 = o1.Parent as Expander;
            o2.IsExpanded = !o2.IsExpanded;
        }

     
        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            var o = sender as RadioButton;
            var o1 = o.CommandParameter;
            Uri roleListViewUri = new Uri(o1.ToString(), UriKind.Relative);
            var sss=  AppEx.Container.GetInstance<UserControl>("UserListWindow");
            this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion,  () => {
                return sss;
            });
        }
    }
}
