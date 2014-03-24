using MahApps.Metro.Controls;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
   [Export("UserView", typeof(IBaseView))]
   [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserAddWindow : MetroWindow, IBaseView
    {

       public UserAddWindow()
        {
            InitializeComponent();

        }
        public void Cancel() 
        {
            
            this.DialogResult = false;
            this.Close();
        }

        public void CloseView()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
