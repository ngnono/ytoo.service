using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job
{
    [DisallowConcurrentExecution]
    public class CouponStatusSyncJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            var awsHost = ConstructRestRequest(context);
            JobDataMap data = context.JobDetail.JobDataMap;
             var interval = data.ContainsKey("interval") ? data.GetIntValue("interval") : 24 * 60;
            var benchDate = DateTime.Now.AddMinutes(-interval);
            var client = new HttpClient();
            var bodydata = JsonConvert.SerializeObject(new
            {
                data = new
                {
                    benchdate = benchDate.ToString("yyyy-MM-ddThh:mm:ssZ")
                }
            });
            HttpContent requestBody = new StringContent(bodydata);
            requestBody.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(awsHost,requestBody);
            var body = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(body);
            if (jsonResponse == null ||
                jsonResponse.isSuccessful != true)
            {
                log.Info("request error" + body);
                return;
            }
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                foreach (var dynamicObject in jsonResponse.data)
                {
                    try
                    {
                        string code = dynamicObject.code;
                        switch ((int)dynamicObject.coupontype)
                        {
                            case 1:
                                var coupon = db.StoreCoupons.Where(s => s.Code == code && s.Status != (int)CouponStatus.Deleted).FirstOrDefault();
                                if (coupon != null)
                                {
                                    coupon.Status = (int)CouponStatus.Used;
                                    coupon.UpdateDate = DateTime.Now;
                                    coupon.UpdateUser = 0;
                                    db.Entry(coupon).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                break;
                            case 2:
                                 var coupon2 = db.CouponHistories.Where(s => s.CouponId == code && s.Status != (int)CouponStatus.Deleted).FirstOrDefault();
                                 if (coupon2 != null)
                                {
                                    coupon2.Status = (int)CouponStatus.Used;
                                    db.Entry(coupon2).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                }
            }

        }
        private string ConstructRestRequest(IJobExecutionContext context)
        {
            JobDataMap data = context.JobDetail.JobDataMap;
            var host = data.GetString("awshost");

                
            var public_key = data.GetString("publickey");
            var private_key = data.GetString("privatekey");

            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("key", public_key);
            query.Add("nonce", new Random(1000).Next().ToString());
            query.Add("timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"));
            var signingValue = new StringBuilder();
            var signedValue = string.Empty;
            foreach (var s in query.Values.ToArray().OrderBy(s => s))
                signingValue.Append(s);
            ILog log = LogManager.GetLogger(this.GetType());
            log.Info(string.Format("signed value:{0}",signingValue.ToString()));
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(private_key)))
            {
               var hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(signingValue.ToString()));
               signedValue =Convert.ToBase64String(hashValue);
               log.Info(signedValue);
                //signedValue = hashValue.Aggregate(new StringBuilder(), (s, e) => s.AppendFormat("{0:x2}",e), s => s.ToString() );
            }
            query.Add("sign",signedValue);
            var requestUrl = new StringBuilder();
            requestUrl.Append(host);
            requestUrl.Append("?");
            return query.Keys.Aggregate(requestUrl, (s, e) => s.AppendFormat("&{0}={1}", e, HttpUtility.UrlEncode(query[e])), s => s.ToString());

        }
    }
}
