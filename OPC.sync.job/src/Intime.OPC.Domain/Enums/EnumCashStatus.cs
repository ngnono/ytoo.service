
using System.ComponentModel;

namespace Intime.OPC.Domain.Enums
{
    /// <summary>
    ///     收银状态
    /// </summary>
    public enum EnumCashStatus
    {
        /// <summary>
        ///     The no cash
        /// </summary>
        [Description("未送收银")] NoCash = 0,

        /// <summary>
        ///     The cash over
        /// </summary>
        [Description("完成收银")] Cashed = 10

    }
}