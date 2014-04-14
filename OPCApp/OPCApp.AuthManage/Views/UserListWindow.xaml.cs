﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using CustomControlLibrary;
using OPCApp.AuthManage.ViewModels;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    ///     RoleWindow.xaml 的交互逻辑
    /// </summary>
    [Export("UserListWindow", typeof (IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserListWindow : UserControl, IBaseView
    {
        public UserListWindow()
        {
            InitializeComponent();
        }

        public void CloseView()
        {
        }

        public void Cancel()
        {
        }

        public bool? ShowDialog()
        {
            return false;
        }

        public void Query(int size, int pageIndex)
        {
            var vm = AppEx.Container.GetInstance<IViewModel>("UserListViewModel") as UserListWindowViewModel;
            vm.PageIndex = pageIndex;
            vm.PageSize = size;
            vm.SearchAction();
        }

        private void dataPager_PageChanged(object sender, PageChangedEventArgs args)
        {
            Query(args.PageSize, args.PageIndex);
        }
    }
}