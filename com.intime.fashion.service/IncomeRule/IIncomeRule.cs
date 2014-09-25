﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.service.IncomeRule
{
    interface IIncomeRule
    {
        decimal Compute(int ruleId, decimal price, int quantity);

        decimal Multiple(int ruleId, decimal baseIncome);
    }
}
