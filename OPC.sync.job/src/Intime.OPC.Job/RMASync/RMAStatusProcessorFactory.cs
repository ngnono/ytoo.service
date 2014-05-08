using Intime.OPC.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.RMASync
{
    public class RMAStatusProcessorFactory
    {
        public static AbstractRMASaleStatusProcessor Create(int status)
        {
            switch (status)
            {
                case 31:
                    return new CashedRNASaleStatusProcessor(EnumRMAStatus.None);
                case 32:
                    return new ShoppingGuidePickUpRMASaleStatusProcessor(EnumRMAStatus.ShoppingGuideReceive);
                default: return new NoneOperationRMAStatusProcessor(EnumRMAStatus.None);
            }
        }
    }
}
