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

         ICommand OKCommand { get; set; }

         ICommand CancelCommand { get; set; }

         T Model { get; set; }
     
    }
}
