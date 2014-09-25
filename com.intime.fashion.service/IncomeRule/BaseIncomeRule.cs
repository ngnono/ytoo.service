using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.service.IncomeRule
{
   public class BaseIncomeRule:IIncomeRule
    {
        protected DbContext _db = null;
        public BaseIncomeRule() {
            _db = ServiceLocator.Current.Resolve<DbContext>();
        }
        public RuleContext Context
        {
            get;
            set;
        }
        public virtual decimal Compute(int ruleId, decimal price, int quantity)
        {
            throw new NotImplementedException();
        }

        public virtual decimal Multiple(int ruleId, decimal baseIncome)
        {
            throw new NotImplementedException();
        }
    }
}
