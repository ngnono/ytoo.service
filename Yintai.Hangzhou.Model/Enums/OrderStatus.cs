using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Model.Enums
{
    public enum OrderStatus
    {
        [Description("已创建")]
        Create = 0,
        [Description("取消")]
        Void = -10,
        [Description("专柜已修改")]
        AgentConfirmed = 1,
        [Description("用户确认")]
        CustomerConfirmed = 2,
        [Description("订单已打印")]
        OrderPrinted = 3,
        [Description("发货单已打印")]
        PreparePack = 4,
        [Description("已发货")]
        Shipped = 5,
        [Description("用户已签收")]
        CustomerReceived = 6,
        [Description("转集团销售")]
        Convert2Sales = 7,
        [Description("用户拒收")]
        CustomerRejected = 10
    }
}
