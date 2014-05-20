using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Intime.OPC.Infrastructure.Mvvm.Utility
{
    public class MvvmUtility
    {
        public static void OnException(Exception exception)
        {
            Action action = () => { MessageBox.Show(exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error); };

            PerformActionOnUIThread(action);
        }

        public static void PerformAction(Action action)
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

        public static void PerformActionOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
