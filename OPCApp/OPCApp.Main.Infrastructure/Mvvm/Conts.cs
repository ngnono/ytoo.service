using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Mvvm
{
    public class Common
    {
        public static string ViewModelKey(string key)
        {
            return string.Format("{0}ViewModel", key);
        }

    }
}
