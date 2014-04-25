using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Service.Logic.IncomeRule
{
    interface IIncomeRule
    {
        decimal Compute(int ruleId, decimal price, int quantity);
    }
}
