using Microsoft.Practices.Prism.Commands;
using OPCApp.Main.Infrastructure.Mvvm.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OPCApp.Infrastructure.Mvvm
{
    public interface IViewModel<T>
    {

        DelegateCommand OKCommand { get; set; }

        DelegateCommand CancelCommand { get; set; }

         T Model { get; set; }

         IBaseView View { get; set; }
     
    }
}
