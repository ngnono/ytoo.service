using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Main.Infrastructure.Mvvm.View
{
    public  interface IBaseView
    {
        void Close();

        void Cancel();

        object DataContext { set; get ; }
    }
}
