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

using System.Windows;
using System.Windows.Controls.Primitives;

namespace OPCApp.Main.Controls
{
    /// <summary>
    ///     Interaction logic for InfoTipToggleButton.xaml
    /// </summary>
    public partial class InfoTipToggleButton : ToggleButton
    {
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register("PopupProperty", typeof (Popup), typeof (InfoTipToggleButton));

        public InfoTipToggleButton()
        {
            InitializeComponent();

            IsThreeState = false;
            Checked += InfoTipToggleButton_Checked;
            Unchecked += InfoTipToggleButton_Checked;
        }

        public Popup Popup
        {
            get { return (Popup) GetValue(PopupProperty); }
            set { SetValue(PopupProperty, value); }
        }

        private void InfoTipToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Popup != null)
            {
                if (IsChecked.HasValue && IsChecked.Value)
                {
                    Popup.PlacementTarget = this;
                    Popup.Placement = PlacementMode.Bottom;
                    Popup.IsOpen = true;
                }
                else
                {
                    Popup.IsOpen = false;
                }
            }
        }
    }
}