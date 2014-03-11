using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MahApps.Metro.Controls;

namespace OPCApp.Main.Infrastructure.Mvvm.View
{
    public  class BaseView:MetroWindow, IBaseView
    {
        public void Close()
        {
            var error = ValidModel();
            if (!string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show(error);
                return;
            }
            DialogResult = true;
           base.Close();
        }

        public void Cancel()
        {
            DialogResult = false;
            base.Close();
        }

        protected virtual string ValidModel()
        {
            return "";
        }


    }
}
