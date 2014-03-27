using System.ComponentModel.Composition;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.ViewModels;

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