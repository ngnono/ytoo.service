using Common.Logging;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.DTO;
using Intime.OPC.Job.Product.ProductSync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.RMASync
{
    public class CashedRMASaleStatusProcessor : AbstractRMASaleStatusProcessor
    {
         private static readonly ILog Log = LogManager.GetCurrentClassLogger();
         public CashedRMASaleStatusProcessor(EnumRMAStatus status) : base(status) { }

        /// <summary>
         /// OPC_SaleRMA  RMACashStatus 入收银状态 Status 退货单状态
         /// OPC_RMA    RMACashNum 、RMACashDate、Status 退货单状态
        /// </summary>
        /// <param name="saleOrderNo"></param>
        /// <param name="statusResult"></param>
        public override void Process(string rmaNo, OrderStatusResultDto statusResult)
        {


            using (var db = new YintaiHZhouContext())
            {
                var saleRMA = db.OPC_SaleRMA.FirstOrDefault(t => t.RMANo == rmaNo);
                saleRMA.RMACashStatus = GetEnumDescription<EnumCashStatus>(EnumCashStatus.Cashed);
                saleRMA.UpdatedDate = DateTime.Now;
                saleRMA.Status = (int)_status;
                saleRMA.UpdatedUser = -100;

                var opcRMA = db.OPC_RMA.FirstOrDefault(t=>t.RMANo==rmaNo );
                opcRMA.RMACashDate = statusResult.PosTime;
                opcRMA.RMACashNum = statusResult.PosSeqNo;
                opcRMA.UpdatedDate = DateTime.Now;
                opcRMA.UpdatedUser = -100;
                opcRMA.Status = (int)_status;

                db.SaveChanges();

                if (string.IsNullOrEmpty(statusResult.Products_SaleCodes))
                {
                    Log.Error("没有销售码信息");
                    return;
                }

                Log.Error("**************************************");
                Log.ErrorFormat(statusResult.Products_SaleCodes);
                Log.Error("**************************************");

                var slices = ParseProductIdAndPosCode(statusResult.Products_SaleCodes);

                foreach (var slice in slices)
                {
                    var productId = slice.Key;
                    var detail =
                        db.OPC_RMADetail.Where(x => x.RMANo == rmaNo)
                            .Join(db.OPC_Stock.Where(s => s.SourceStockId == productId), d => d.StockId, s => s.Id,
                                (o, s) => o)
                            .FirstOrDefault();
                    if (detail != null)
                    {
                        detail.SectionCode = slice.Value;
                        db.SaveChanges();
                    }
                }
            }
        }


        /// <summary>
        /// 返回指定枚举值的描述（通过 
        /// <see cref="System.ComponentModel.DescriptionAttribute"/> 指定）。
        /// 如果没有指定描述，则返回枚举常数的名称，没有找到枚举常数则返回枚举值。
        /// </summary>
        /// <param name="value">要获取描述的枚举值。</param>
        /// <returns>指定枚举值的描述。</returns>

        public static string GetEnumDescription<TEnum>(object value)
        {
            Type enumType = typeof(TEnum);

            var name = Enum.GetName(enumType, Convert.ToUInt32(value));
            if (name == null)
                return string.Empty;

            object[] obj = enumType.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute),false);
            if (obj == null || obj.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                DescriptionAttribute attr = obj[0] as DescriptionAttribute;
                return attr.Description;
            }

        }

        /// <summary>
        /// 解析信息部给的结构
        /// </summary>
        /// <param name="strPosSeq">信息部给的结构 : productid|comcode,productid|comcode</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> ParseProductIdAndPosCode(string strPosSeq)
        {
            var slices = strPosSeq.Split(',');
            return from slice in slices select slice.Split('|') into kv where kv.Length == 2 select new KeyValuePair<string, string>(kv[0],kv[1]);
        }
    }

}
