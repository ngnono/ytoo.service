﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Model.Enums
{
    public enum RMAStatus
    {
        [Description("已创建")]
        Created = 1,
        [Description("审核通过")]
        PassConfirmed = 2,
        [Description("用户确认")]
        CustomerConfirmed = 3,
        [Description("已签收")]
        PackageReceived = 12,
        [Description("完成")]
        Complete = 10,
        [Description("审核不通过返还客户")]
        Reject2Customer = 13,
        [Description("打印退货单并退款")]
        PrintRMA = 14,
        [Description("取消")]
        Void = -10,
        Reject
    }
}
