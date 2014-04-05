using System.ComponentModel.Composition;
using CustomControlLibrary;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.ViewModels;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.Views
{
    [Export("UsersView", typeof (UsersWindow))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UsersWindow : MetroWindow

    {
        public UsersWindow()
        {
            InitializeComponent();
        }

        [Import("UsersViewModel", typeof (UsersWindowViewModel))]
        public UsersWindowViewModel ViewModel
        {
            get { return DataContext as UsersWindowViewModel; }
            set
            {
                DataContext = value;
                ViewModel.CancelCommand = new DelegateCommand(Cancel);
                ViewModel.OkCommand = new DelegateCommand(CloseView);
            }
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