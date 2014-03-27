using System;
using System.Windows.Input;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.Infrastructure.Mvvm
{
    public interface IViewModel
    {
        IBaseView View { get; set; }

        Object Model { get; set; }
    }

    public interface ISubView
    {
        ICommand OKCommand { get; set; }

        ICommand CancelCommand { get; set; }
    }
}