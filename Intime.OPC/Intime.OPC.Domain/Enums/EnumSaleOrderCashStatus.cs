using System.ComponentModel;

namespace Intime.OPC.Domain.Enums
{
    public enum EnumSaleOrderCashStatus
    {
        /// <summary>
        ///     The no cash
        /// </summary>
        [Description("未送收银")] NoCash = 0,

        /// <summary>
        ///     The send cash
        /// </summary>
        [Description("已送收银")] SendCash = 5,

        /// <summary>
        ///     The cash over
        /// </summary>
        [Description("完成收银")] CashOver = 10
    }
}