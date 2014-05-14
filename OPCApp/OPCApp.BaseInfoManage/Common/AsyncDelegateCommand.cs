using CustomControlLibrary;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Intime.OPC.Modules.Dimension.Common
{
    //ToDo: Refactoring is required to remove the duplicated codes between the two classes
    public class AsyncDelegateCommand<T> : ICommand
    {
        private Action<T> executeMethod;
        private Func<T, bool> canExecuteMethod;

        public AsyncDelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public AsyncDelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (canExecuteMethod != null && !canExecuteMethod((T)parameter)) return;

            ProgressBarWindow progressDialog = new ProgressBarWindow();
            Task task = new Task(() => 
            { 
                executeMethod((T)parameter);
                progressDialog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate 
                    {
                        progressDialog.Hide(); 
                    }));
            });

            task.Start();
            progressDialog.ShowDialog();
        }
    }

    public class AsyncDelegateCommand : ICommand
    {
        private Action executeMethod;
        private Func<bool> canExecuteMethod;

        public AsyncDelegateCommand(Action executeMethod)
            :this(executeMethod, null)
        {
        }


        public AsyncDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (canExecuteMethod != null && !canExecuteMethod()) return;

            ProgressBarWindow progressDialog = new ProgressBarWindow();
            Task task = new Task(() =>
            {
                executeMethod();
                progressDialog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    progressDialog.Hide();
                }));
            });

            task.Start();
            progressDialog.ShowDialog();
        }
    }
}
