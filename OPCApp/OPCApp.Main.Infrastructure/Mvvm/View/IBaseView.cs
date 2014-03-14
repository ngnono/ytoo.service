using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Mvvm.View
{
    public  interface IBaseView
    {
        void CloseView();

        void Cancel();
        bool? ShowDialog();
        object DataContext { set; get ; }
    }
}
