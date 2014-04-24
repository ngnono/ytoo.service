﻿using System.ComponentModel.Composition;
using CustomControlLibrary;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.ViewModels;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    [Export("UsersView", typeof (UsersWindow))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UsersWindow : MetroWindow, IBaseView

    {
        [ImportingConstructor]
        public UsersWindow(UsersWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            ViewModel.CancelCommand = new DelegateCommand(Cancel);
            ViewModel.OkCommand = new DelegateCommand(CloseView);
        }


        public UsersWindowViewModel ViewModel
        {
            get { return DataContext as UsersWindowViewModel; }
            set { DataContext = value; }
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

        public void Query(int size, int pageIndex)
        {
            ViewModel.PageIndex = pageIndex;
            ViewModel.PageSize = size;
            ViewModel.SearchCommand.Execute();
        }

        private void dataPager_PageChanged(object sender, PageChangedEventArgs args)
        {
            Query(args.PageSize, args.PageIndex);
        }
    }
}