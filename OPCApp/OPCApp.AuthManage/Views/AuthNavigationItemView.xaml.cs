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

//.Regions;
//.Infrastructure;

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using OPCApp.AuthManage.ViewModels;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.Views
{
    [Export]
    public partial class AuthNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        [Import] public IRegionManager regionManager;

        [ImportingConstructor]
        public AuthNavigationItemView(AuthNavaeigationItemViewModel viewModel)
        {
            InitializeComponent();
            ViewMode = viewModel;
            ViewMode.ClickCommand = new DelegateCommand(clickCommand);
            ViewMode.MenuClickCommand = new DelegateCommand<string>(menuClickCommand);
        }

        public AuthNavaeigationItemViewModel ViewMode
        {
            get { return DataContext as AuthNavaeigationItemViewModel; }
            set { DataContext = value; }
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += MainContentRegion_Navigated;
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            UpdateNavigationButtonState(e.Uri);
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
            var uri = new Uri(url, UriKind.Relative);
            regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var o = sender as TextBlock;
            var o1 = o.Parent as StackPanel;
            var o2 = o1.Parent as Expander;
            o2.IsExpanded = !o2.IsExpanded;
        }


        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var o = sender as RadioButton;
                object o1 = o.CommandParameter;
                while (regionManager.Regions[RegionNames.MainContentRegion].Views.Count() > 0)
                {
                    regionManager.Regions[RegionNames.MainContentRegion].Remove(
                        regionManager.Regions[RegionNames.MainContentRegion].Views.FirstOrDefault());
                }
                //this.regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion,);
                if (o1.ToString().Contains("ViewModel")) //待调整
                {
                    regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, () =>
                        AppEx.Container.GetInstance<IViewModel>(o1.ToString()).View
                        );
                }
                else
                {
                    regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion,
                        () => AppEx.Container.GetInstance<UserControl>(o1.ToString())
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("功能正在开发中", "提示");
            }
        }
    }
}