using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Service.Logic
{
    public interface IDbAware
    {
        DbContext GetContext();
    }
}
