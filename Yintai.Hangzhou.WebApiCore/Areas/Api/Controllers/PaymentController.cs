using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Com.Alipay;
using System.Data.Entity;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using System.Transactions;
using Yintai.Architecture.Common.Data.EF;
using Newtonsoft.Json;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;


namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class PaymentController : Controller
    {
        private IEFRepository<OrderTransactionEntity> _orderTranRepo;
        private IEFRepository<PaymentNotifyLogEntity> _paymentNotifyRepo;
        private IOrderRepository _orderRepo;
        private IOrderLogRepository _orderlogRepo;

        public PaymentController(IEFRepository<OrderTransactionEntity> ordTranRepo
            , IEFRepository<PaymentNotifyLogEntity> paynotiRepo
            , IOrderRepository orderRepo
            , IOrderLogRepository orderLogRepo)
        {
            _orderTranRepo = ordTranRepo;
            _paymentNotifyRepo = paynotiRepo;
            _orderRepo = orderRepo;
            _orderlogRepo = orderLogRepo;
        }

        public ActionResult Notify()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)
                {
                    string out_trade_no = Request.Form["out_trade_no"];
                    string trade_no = Request.Form["trade_no"];

                    string trade_status = Request.Form["trade_status"];

                    var amount = decimal.Parse(Request.Form["total_fee"]);
                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        var paymentEntity = Context.Set<OrderTransactionEntity>().Where(p => p.OrderNo == out_trade_no).FirstOrDefault();
                        var orderEntity = Context.Set<OrderEntity>().Where(o => o.OrderNo == out_trade_no).FirstOrDefault();
                        if (paymentEntity == null && orderEntity != null)
                        {
                            using (var ts = new TransactionScope())
                            {
                                _paymentNotifyRepo.Insert(new PaymentNotifyLogEntity()
                                {
                                    CreateDate = DateTime.Now,
                                    OrderNo = out_trade_no,
                                    PaymentCode = Config.PaymentCode,
                                    PaymentContent = JsonConvert.SerializeObject(sPara)
                                });

                                _orderTranRepo.Insert(new OrderTransactionEntity()
                                {
                                    Amount = amount,
                                    OrderNo = out_trade_no,
                                    CreateDate = DateTime.Now,
                                    IsSynced = false,
                                    PaymentCode = Config.PaymentCode,
                                    TransNo = trade_no
                                });
                                if (orderEntity.Status != (int)OrderStatus.Paid)
                                {
                                    orderEntity.Status = (int)OrderStatus.Paid;
                                    orderEntity.UpdateDate = DateTime.Now;
                                    orderEntity.RecAmount = amount;
                                    _orderRepo.Update(orderEntity);

                                    _orderlogRepo.Insert(new OrderLogEntity()
                                    {
                                        CreateDate = DateTime.Now,
                                        CreateUser = 0,
                                        CustomerId = orderEntity.CustomerId,
                                        Operation = "支付订单",
                                        OrderNo = out_trade_no,
                                        Type = (int)OrderOpera.CustomerPay
                                    });
                                }
                                ts.Complete();
                            }
                        }

                    }
                    return Content("success");

                }
                else
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }

        private SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }
        private DbContext Context
        {
            get
            {
                return ServiceLocator.Current.Resolve<DbContext>();
            }
        }
    }
}
