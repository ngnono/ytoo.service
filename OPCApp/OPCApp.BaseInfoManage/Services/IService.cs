using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Services
{
    public interface IService<T>
    {
        void Create(T obj);

        void Update(T obj);

        void Delete(int id);

        IEnumerable<T> Query(string name);

        IEnumerable<T> QueryAll();
    }
}
