using Common.Logging;
using Intime.O2O.ApiClient.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.Models;
using Intime.OPC.Job.Order.Repository;
using Intime.OPC.Job.Product.ProductSync;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Job.Order.OrderStatusSync
{

    [DisallowConcurrentExecution]
    public class OrderStatusSync : IJob
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private DateTime benchTime = DateTime.Now.AddMinutes(-20);
        private readonly IRemoteRepository _remoteRepository=new RemoteRepository();

        private void DoQuery(Action<IQueryable<Intime.OPC.Domain.Models.OPC_Sale>> callback)
        {
            using (var context = new YintaiHZhouContext())
            {
                var minx = context.OPC_Sale.Where(t => t.UpdatedDate > benchTime && t.Status > 0 ).Min(t=>t.Status);
                var linq = context.OPC_Sale.Where(t => t.UpdatedDate > benchTime && t.Status==minx).AsQueryable();
                
                if (callback != null)
                    callback(linq);
            }
        }

        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var totalCount = 0;

            if (!isRebuild)
                benchTime = data.GetDateTime("benchtime");

            DoQuery(skus =>
            {
                totalCount = skus.Count();
            });
            int cursor = 0;
            int size = 20;
            while (cursor < totalCount)
            {
                List<Intime.OPC.Domain.Models.OPC_Sale> oneTimeList = null;
                DoQuery(r => oneTimeList = r.OrderBy(t => t.OrderNo).Skip(cursor).Take(size).ToList());
                foreach (var opc_sale in oneTimeList)
                {
                    Process(opc_sale);
                    ProcessToProduct(opc_sale);
                }
                cursor += size;
            }


        }

        /// <summary>
        /// 同步订单
        /// </summary>
        /// <param name="opc_sale"></param>
        private void ProcessToProduct(Domain.Models.OPC_Sale opc_sale)
        {
            OPC_SaleDetail opc_SaleDetail;
            Intime.OPC.Domain.Models.Section section;
            //Intime.OPC.Domain.Models
            using (var db = new YintaiHZhouContext())
            {
                opc_SaleDetail = db.OPC_SaleDetail.FirstOrDefault(t => t.SaleOrderNo == opc_sale.SaleOrderNo);
                section = db.Sections.FirstOrDefault(a => a.Id == opc_sale.SectionId);
                //order = db.Orders.FirstOrDefault(b => b.OrderNo == opc_sale.OrderNo);
            }
            OrderStatusDetail opc_saleorderStatusDetail = new OrderStatusDetail
            {
                Id = opc_sale.OrderNo,
                Status = opc_sale.Status,
                Head = new HeadDto
                {
                    Id = opc_sale.OrderNo,
                    MainId = opc_sale.OrderNo,
                    Flag = 0,
                    CreateTime = opc_sale.CreatedDate,
                    PayTime = opc_sale.CashDate??DateTime.Now,
                    Type = opc_sale.SalesType,
                    Status = opc_sale.Status,
                    Quantity = opc_sale.SalesCount,
                    Discount = 0,
                    Total = opc_sale.SalesAmount,
                    VipNo = "",
                    VipMemo = "",
                    StoreNo = section.StoreCode,
                    OldId = "",
                    OperId = opc_sale.CreatedUser.ToString(),
                    OperName = "",
                    OperTime = opc_sale.CreatedDate
                },
                Detail = new DetailDto
                {
                    Id = opc_sale.OrderNo,
                    ProductId = "",
                    ProductName = "",
                    Price = opc_SaleDetail.Price,
                    Discount = opc_SaleDetail.Price,
                    VipDiscount = 0,
                    Quantity = opc_SaleDetail.SaleCount,
                    Total = opc_SaleDetail.Price,
                    RowNo = 1,
                    ComCode = "",
                    Counter = "",
                    Memo = "",
                    StoreNo = section.StoreCode
                },
                PayMent = new PayMentDto
                {
                    Id = opc_sale.OrderNo,
                    Type = "",
                    TypeId = 25,//order.PaymentMethodCode,
                    TypeName ="",
                    No = "",
                    Amount = opc_sale.SalesAmount,
                    RowNo = 1,
                    Memo = "",
                    StoreNo = section.StoreCode
                }

            };
            var dataResult = _remoteRepository.GetOderStatus(opc_saleorderStatusDetail);
            if (dataResult==null)
            {
                var res = dataResult.desc;
                Log.InfoFormat("订单信息查询失败,orderNo:{0},status:{1}", opc_sale.OrderNo,opc_sale.Status);
            }

        }
        private void Process(Domain.Models.OPC_Sale opc_sale)
        {
            using (var db = new YintaiHZhouContext())
            {
                var p = db.Orders.FirstOrDefault(t => t.OrderNo == opc_sale.OrderNo);
                switch(opc_sale.Status)
                {
                    case 0:
                        p.Status = 0;
                        break;
                    case 5:
                        break;
                    case 10:
                        p.Status = 10;
                        break;
                    case 15:
                        break;
                    case 20:
                        p.Status = 20;
                        break;
                    case 25:
                        p.Status = 25;
                        break;
                    case 30:
                        p.Status = 30;
                        break;
                    case 35:
                        p.Status = 350;
                        break;
                    case 40:
                        p.Status = 40;
                        break;
                    default:
                        p.Status = opc_sale.Status;
                        break;

                }

                p.UpdateDate = DateTime.Now;
                p.UpdateUser = SystemDefine.SystemUser;
                db.SaveChanges();
                Log.InfoFormat("完成订单状态更新,orderNo:{0},status:{1}", p.OrderNo, SystemDefine.OrderFinishSplitStatusCode);

            }
        }

        #endregion
    }
}
