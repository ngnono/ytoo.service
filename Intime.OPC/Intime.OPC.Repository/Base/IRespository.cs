using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository.Base
{
    public interface IRespository<T> where T:new()
    {
        bool Create(T role);
        bool Update(T role);
        bool Delete(int roleId);
        IList<T> Select();
    }
}
