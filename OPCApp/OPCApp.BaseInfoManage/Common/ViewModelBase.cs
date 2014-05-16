using Intime.OPC.Infrastructure.Validation;
using Intime.OPC.Modules.Dimension.Properties;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Intime.OPC.Modules.Dimension.Common
{
    public class ViewModelBase : ValidatableBindableBase
    {
        protected void OnException(Exception exception)
        {
            Action action = () => { MessageBox.Show(exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error); };

            PerformActionOnUIThread(action);
        }

        protected void PerformAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
        }

        protected void PerformActionOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
