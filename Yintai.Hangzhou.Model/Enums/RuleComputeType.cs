using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Model.Enums
{
    public enum RuleComputeType
    {
        //compute income directly
        Compute= 1,
        //compute income based on other rules
        Multiple = 2
    }
}
