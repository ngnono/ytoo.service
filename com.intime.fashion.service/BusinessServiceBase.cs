using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.service
{
    public abstract class BusinessServiceBase:IDbAware,IDebugAare
    {
        protected ILog _log ;
        protected DbContext _db ;

        public BusinessServiceBase()
            : this(ServiceLocator.Current.Resolve<ILog>(),
            ServiceLocator.Current.Resolve<DbContext>())
        { }
        public BusinessServiceBase(ILog log,
            DbContext db)
        {
            _db = db;
            _log = log;
        }

        public ILog GetLog()
        {
            return _log;
        }

        public DbContext GetContext()
        {
            return _db;
        }
    }
}
