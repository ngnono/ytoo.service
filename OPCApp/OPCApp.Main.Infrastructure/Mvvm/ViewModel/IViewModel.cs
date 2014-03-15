using Microsoft.Practices.Prism.Commands;
using OPCApp.Infrastructure.Mvvm.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
