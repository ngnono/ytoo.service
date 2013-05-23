using com.intime.fashion.common;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job
{
    [DisallowConcurrentExecution]
    class ExpireStoreCouponReturnPointsJob:IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            var privatekey = data.GetString("privatekey");
            var publickey = data.GetString("publickey");
            var awsExpireurl = data.GetString("awsexpireurl");
            int point2GroupRatio = int.Parse(ConfigurationManager.AppSettings["point2groupratio"]);
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            DateTime fromDate =  DateTime.Today.AddDays(-1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                      var coupons = from p in db.StoreCoupons
                                  where p.ValidEndDate >=fromDate && p.ValidEndDate < DateTime.Today
                                    && p.Status == (int)CouponStatus.Normal
                                   select p;
                           
                int totalCount = coupons.Count();
                while (cursor < totalCount)
                {
                    foreach (var coupon in coupons.OrderBy(c=>c.Id).Skip(cursor).Take(size))
                    {
                      bool canRebate =  AwsHelper.SendHttpMessage(awsExpireurl, new { code = coupon.Code }
                            , publickey
                            , privatekey
                            , null
                            ,null);
                      if (canRebate)
                      {
                          var point = db.PointHistories.Where(u => u.User_Id == coupon.UserId.Value && u.Type== (int)PointType.VoidCoupon && u.Description==coupon.Code).FirstOrDefault();
                          if (point == null)
                          {
                              db.PointHistories.Add(new PointHistoryEntity()
                              {
                                  Amount = (decimal)coupon.Points * point2GroupRatio,
                                  CreatedDate = DateTime.Now,
                                  CreatedUser = 0,
                                  Description = coupon.Code,
                                  Name = string.Format("代金券过期返回积点{0}", coupon.Points * point2GroupRatio),
                                  PointSourceType = (int)PointSourceType.System,
                                  PointSourceId = 0,
                                  Status = (int)DataStatus.Normal,
                                  Type = (int)PointType.VoidCoupon,
                                  UpdatedDate = DateTime.Now,
                                  UpdatedUser = 0,
                                  User_Id = coupon.UserId.Value

                              });
                              db.SaveChanges();
                              successCount++;
                          }
                          
                      }
                     
                    }
                    cursor += size;
 
                }
            }
            sw.Stop();
            log.Info(string.Format("{0} rebate points in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }
    }
}
