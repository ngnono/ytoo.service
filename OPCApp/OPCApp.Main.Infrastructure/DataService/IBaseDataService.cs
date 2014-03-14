using OPCApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.DataService
{
    public interface  IBaseDataService <T>
    {
        ResultMsg Add(T model);
        ResultMsg Edit(T model);
        ResultMsg Delete(T model);


        PageResult<T> Search(IFilter filter);
    }
}
