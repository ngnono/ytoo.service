using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Service
{
    public interface IService
    {
       
    }

    public interface ICanAdd<T>
    {
        bool Add(T t);
    }

    public interface ICanUpdate<T>
    {
        bool Update(T t);
    }

    public interface ICanDelete
    {
        bool DeleteById(int id);
    }

}
