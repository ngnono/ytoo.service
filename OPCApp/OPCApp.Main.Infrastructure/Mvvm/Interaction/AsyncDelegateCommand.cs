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

namespace Intime.OPC.Infrastructure.Mvvm
{
    //ToDo: Refactoring is required to remove the duplicated codes between the two classes
    public class AsyncDelegateCommand<T> : ICommand
    {
        private Action<T> executeMethod;
        private Func<T, bool> canExecuteMethod;
        private Action<Exception> errorHandler;

        public AsyncDelegateCommand(Action<T> executeMethod, Action<Exception> errorHandler)
            : this(executeMethod,errorHandler, null)
        {
        }

        public AsyncDelegateCommand(Action<T> executeMethod, Action<Exception> errorHandler, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.errorHandler = errorHandler;
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
                try
                {
                    executeMethod((T)parameter);
                }
                catch (Exception exception)
                {
                    if (errorHandler != null) errorHandler(exception);
                }
                finally
                {
                    CloseDialog(progressDialog);
                }
                
            });

            task.Start();
            progressDialog.ShowDialog();
        }

        private void CloseDialog(Window dialog)
        {
            dialog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                dialog.Hide();
                dialog.Close();
            }));
        }
    }

    public class AsyncDelegateCommand : ICommand
    {
        private Action executeMethod;
        private Func<bool> canExecuteMethod;
        private Action<Exception> errorHandler;

        public AsyncDelegateCommand(Action executeMethod, Action<Exception> errorHandler)
            :this(executeMethod,errorHandler, null)
        {
        }

        public AsyncDelegateCommand(Action executeMethod, Action<Exception> errorHandler, Func<bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.errorHandler = errorHandler;
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
                try
                {
                    executeMethod();
                }
                catch (Exception exception)
                {
                    if (errorHandler != null) errorHandler(exception);
                }
                finally
                {
                    CloseDialog(progressDialog);
                }
            });

            task.Start();
            progressDialog.ShowDialog();
        }

        private void CloseDialog(Window dialog)
        {
            dialog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                dialog.Hide();
                dialog.Close();
            }));
        }
    }
}
