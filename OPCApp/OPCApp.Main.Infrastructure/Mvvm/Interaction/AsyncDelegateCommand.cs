using CustomControlLibrary;
using Intime.OPC.Infrastructure.Mvvm.Utility;
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
        private Action<T> _executeMethod;
        private Func<T, bool> _canExecuteMethod;
        private Action<Exception> _errorHandler;

        public AsyncDelegateCommand(Action<T> executeMethod)
            :this(executeMethod, MvvmUtility.OnException)
        {
        }

        public AsyncDelegateCommand(Action<T> executeMethod, Action<Exception> errorHandler)
            : this(executeMethod,errorHandler, null)
        {
        }

        public AsyncDelegateCommand(Action<T> executeMethod, Action<Exception> errorHandler, Func<T, bool> canExecuteMethod)
        {
            this._executeMethod = executeMethod;
            this._errorHandler = errorHandler;
            this._canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_canExecuteMethod != null && !_canExecuteMethod((T)parameter)) return;

            ProgressBarWindow progressDialog = new ProgressBarWindow();
            Task task = new Task(() => 
            {
                try
                {
                    _executeMethod((T)parameter);
                }
                catch (Exception exception)
                {
                    if (_errorHandler != null) _errorHandler(exception);
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
        private Action _executeMethod;
        private Func<bool> _canExecuteMethod;
        private Action<Exception> _errorHandler;

        public AsyncDelegateCommand(Action executeMethod)
            : this(executeMethod, null, null)
        {
        }

        public AsyncDelegateCommand(Action executeMethod, Action<Exception> errorHandler)
            :this(executeMethod,errorHandler, null)
        {
        }

        public AsyncDelegateCommand(Action executeMethod, Action<Exception> errorHandler, Func<bool> canExecuteMethod)
        {
            this._executeMethod = executeMethod;
            this._errorHandler = errorHandler;
            this._canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_canExecuteMethod != null && !_canExecuteMethod()) return;

            ProgressBarWindow progressDialog = new ProgressBarWindow();
            Task task = new Task(() =>
            {
                try
                {
                    _executeMethod();
                }
                catch (Exception exception)
                {
                    if (_errorHandler == null) throw;

                    _errorHandler(exception);
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
