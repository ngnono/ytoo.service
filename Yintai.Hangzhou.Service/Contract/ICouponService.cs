using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Service.Contract
{
    public interface ICouponService
    {
        /// <summary>
        /// 获取用户优惠列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="sourceId">来源ID</param>
        /// <param name="sourceType">来源类型</param>
        /// <returns></returns>
        List<CouponHistoryEntity> Get(int userId, int sourceId, SourceType sourceType);
    }
}
