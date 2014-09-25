using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.service.IncomeRule
{
    class IncomeRuleMultiple:BaseIncomeRule
    {

        public override decimal Multiple(int ruleId, decimal baseIncome)
        {
            if (Context == null || Context.OrderItem == null)
                return baseIncome;
            var rules = _db.Set<IMS_AssociateIncomeRuleMultipleEntity>()
                                .Where(iai => iai.RuleId == ruleId && iai.Status == (int)DataStatus.Normal);
            var benchTime = Context.OrderItem.CreateDate.TimeOfDay;
            foreach (var rule in rules)
            {
                if (TimeSpan.Compare(rule.EffectTimeFrom, benchTime) <= 0 ||
                     TimeSpan.Compare(rule.EffectTimeTo, benchTime) >= 0)
                {
                    return rule.Multiple * baseIncome;
                }
            }
            return baseIncome;
        }
    }
}
