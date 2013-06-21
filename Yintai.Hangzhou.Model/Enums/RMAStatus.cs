using System;
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
        [Description("已签收")]
        PackageReceived =2,
        [Description("审核不通过返还客户")]
        Reject2Customer =3,
        [Description("打印退货单并退款")]
        PrintRMA = 4,
        [Description("取消")]
        Void = -10
    }
}
