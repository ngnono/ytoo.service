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
using Microsoft.Practices.Prism.Regions;
using OPCApp.Infrastructure;


namespace OPCApp.Order.Views
{
    [Export]
    public partial class OrderNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri calendarViewUri = new Uri("OrderSearch1", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public OrderNavigationItemView()
        {
            InitializeComponent();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            //IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            //if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            //{
            //    mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
            //}
        }

        //public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        //{
        //    this.UpdateNavigationButtonState(e.Uri);
        //}

        //private void UpdateNavigationButtonState(Uri uri)
        //{
        //    this.NavigateToCalendarRadioButton.IsChecked = (uri == calendarViewUri);
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, calendarViewUri);
        }
    }
}
