using com.intime.fashion.common;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.jobscheduler.Job
{
    public class Push2mysqlJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.GetIntValue("intervalofmin");
            var benchDate = DateTime.Now.AddMinutes(-interval);
            var host = data.GetString("awshost");
            var public_key = data.GetString("publickey");
            var private_key = data.GetString("privatekey");

            DoSyncWXReply(host,public_key,private_key,benchDate);
            IndexPromotionCode(benchDate);
            IndexStorePromotionCode(benchDate);
           
        }

        private void DoSyncWXReply(string host, string public_key, string private_key, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from r in db.WXReplies
                            where (r.UpdateDate >= benchDate)
                            select r;

                int totalCount = prods.Count();
                while (cursor < totalCount)
                {
                    var linq = prods.OrderByDescending(p => p.Id).Skip(cursor).Take(size).ToList();
                    foreach (var l in linq)
                    {
                        try
                        {
                           dynamic jsonResponse = null;
                           if (AwsHelper.SendHttpMessage(host, new
                            {
                                id = l.Id,
                                rkey = l.MatchKey,
                                rmsg = l.ReplyMsg,
                                status = l.Status
                            }, public_key, private_key, r => jsonResponse = r, null))
                           {
                               successCount++;
                           }
                           else
                           {
                               log.Info("request error!");
                               continue;
                           }

                            
                        }
                        catch (Exception ex)
                        {
                            log.Info(ex);
                        }
                    }

                    cursor += size;
                }

            }
            sw.Stop();
            log.Info(string.Format("{0} wx rely message in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
            
        }


        private void IndexStorePromotionCode( DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from r in db.StoreCoupons
                            where (r.CreateDate >= benchDate)
                            select r;

                int totalCount = prods.Count();
                while (cursor < totalCount)
                {
                    var linq = prods.OrderByDescending(p => p.Id).Skip(cursor).Take(size).ToList();
                    foreach (var l in linq)
                    {
                        try
                        {
                            AwsHelper.SendMessage(l.TypeName
                                , () => l.Composing());
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            log.Info(ex);
                        }
                    }

                    cursor += size;
                }

            }
            sw.Stop();
            log.Info(string.Format("{0} store codes in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }
        private void IndexPromotionCode(DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from r in db.CouponHistories
                            where (r.CreatedDate >= benchDate)
                            select r;

                int totalCount = prods.Count();
                while (cursor < totalCount)
                {
                    var linq = prods.OrderByDescending(p => p.Id).Skip(cursor).Take(size).ToList();
                    foreach (var l in linq)
                    {
                        try
                        {
                            AwsHelper.SendMessage(l.TypeName
                                , () => l.Composing());
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            log.Info(ex);
                        }
                    }

                    cursor += size;
                }

            }
            sw.Stop();
            log.Info(string.Format("{0} promotion codes in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

    }
}
