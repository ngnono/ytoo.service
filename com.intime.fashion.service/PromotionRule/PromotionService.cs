using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.service.PromotionRule
{
    class PromotionService
    {
       public  IPromotionSharePolicy GetDefaultSharePolicy()
        {
            return new PricePerAmountPolicy();
        }
    }
}
