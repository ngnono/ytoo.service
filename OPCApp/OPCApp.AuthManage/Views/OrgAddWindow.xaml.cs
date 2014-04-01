﻿using System.ComponentModel.Composition;
using MahApps.Metro.Controls;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    [Export("OrgView", typeof (IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OrgAddWindow : MetroWindow, IBaseView
    {
        public OrgAddWindow()
        {
            InitializeComponent();
        }

        public void Cancel()
        {
            DialogResult = false;
            Close();
        }

        public void CloseView()
        {
            DialogResult = true;
            Close();
        }
    }
}