using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Model.Enums
{
    public enum PointType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 注册
        /// </summary>
        Register = 1,

        /// <summary>
        /// 邀请其他好友消费
        /// </summary>
        InviteConsumption = 2,

        /// <summary>
        /// 被邀请消费赠送
        /// </summary>
        BeConsumption = 3,

        /// <summary>
        /// 奖励
        /// </summary>
        Reward = 4,

        /// <summary>
        /// 消费
        /// </summary>
        Consumption
    }
}
