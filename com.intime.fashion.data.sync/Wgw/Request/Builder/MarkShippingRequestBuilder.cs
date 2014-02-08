using com.intime.jobscheduler.Job.Wgw;
using System.Linq;

namespace com.intime.fashion.data.sync.Wgw.Request.Builder
{
    public class MarkShippingRequestBuilder:RequestParamsBuilder
    {
        public MarkShippingRequestBuilder(ISyncRequest request) : base(request)
        {
        }

        public override ISyncRequest BuildParameters(object dealCode)
        {
            Request.Put("dealCode",dealCode);
            Request.Put("markForce",0);
            Request.Put("arriveDays", WgwConfigHelper.ArrivingDays);
            using (var db = GetDbContext())
            {
                var outBoundInfo =
                    db.Map4Orders.Where(
                        m => m.Channel == ConstValue.WGW_CHANNEL_NAME && m.ChannelOrderCode == dealCode.ToString()).Join(db.Orders, m => m.OrderNo, o => o.OrderNo, (o, m) => o).Join(db.Outbounds.Where(o => o.Status == 1),o=>o.OrderNo,ot=>ot.SourceNo,(o,ot)=>ot).FirstOrDefault();
                if (null == outBoundInfo)
                {
                    throw new WgwSyncException(string.Format("未找到渠道订单{0}的发货信息",dealCode));
                }
                var ship = db.ShipVias.FirstOrDefault(s => s.Id == outBoundInfo.ShippingVia.Value);
                if (ship == null)
                {
                    throw new WgwSyncException(string.Format("未找到订单{0}的物流信息,物流公司ID{1}",dealCode,outBoundInfo.ShippingVia.Value));
                }
                Request.Put("wuliuCode",outBoundInfo.ShippingNo);
                Request.Put("wuliuCompany",ship.Name);
                Request.Put("recvAddr",outBoundInfo.ShippingAddress);
                return Request;
            }
        }
    }
}
