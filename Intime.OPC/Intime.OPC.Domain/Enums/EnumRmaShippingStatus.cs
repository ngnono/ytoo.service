using System.ComponentModel;

namespace Intime.OPC.Domain.Enums
{
    /// <summary>
    ///     退货快递单状态
    /// </summary>
    public enum EnumRmaShippingStatus
    {
        [Description("未打印")] NoPrint = 100,

        [Description("已打印")] Printed = 105,

        [Description("打印完成")] PrintOver = 110,
    }
}