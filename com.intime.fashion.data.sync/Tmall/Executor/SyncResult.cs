using System;
using com.intime.fashion.data.tmall.Models;

namespace com.intime.fashion.data.sync.Tmall.Executor
{
    public class SyncResult
    {
        public static SyncResult FailedResult(Exception ex,JDP_TB_TRADE trade)
        {
            return new SyncResult()
            {
                FailedReason =  string.Format("Order tid is({0})",trade.tid),
                Exception =  ex,
                Succeed =  false
            };
        }

        public static SyncResult SucceedResult(string targetOrderNo,decimal tmallOrderId)
        {
            return new SyncResult()
            {
                Succeed = true,
                TargetOrderNo = targetOrderNo,
                TmalOrderId = tmallOrderId
            };
        }

        public decimal TmalOrderId { get; set; }

        public string TargetOrderNo { get; set; }

        public bool Succeed { get; set; }

        public string FailedReason { get; set; }

        public Exception Exception { get; set; }
    }
}