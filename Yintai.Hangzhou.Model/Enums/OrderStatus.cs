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
        [Description("开始打包")]
        PreparePack = 3,
        [Description("已送货")]
        Shipped = 4,
        [Description("用户已签收")]
        CustomerReceived = 5,
        [Description("转集团销售")]
        Convert2Sales = 6,
        [Description("用户拒收")]
        CustomerRejected = 10
    }
}
