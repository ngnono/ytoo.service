using System.Linq;
using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Quartz;
using System;
using System.Threading;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs
{
    public class PropuctPropertySyncJob : IJob
    {
        private const int PageSize = 200;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        readonly RemoteRepository _remoteRepository = new RemoteRepository(new DefaultApiClient());

        public void Execute(IJobExecutionContext context)
        {
#if !DEBUG
            JobDataMap data = context.JobDetail.JobDataMap;
            var interval = data.ContainsKey("intervalofhrs") ? data.GetInt("intervalofhrs") : 1;

            var benchTime = data.GetDateTime("benchdate");
            var lastUpdateDateTime = DateTime.Now.AddHours(-interval);
            this.Sync(benchTime, lastUpdateDateTime);
#else
            this.Sync(new DateTime(2014, 4, 22), DateTime.Now.AddMonths(-1));
#endif
        }

        public void Sync(DateTime benchDate, DateTime lastUpdateDateTime)
        {
            var pageIndex = 1;

            while (true)
            {
                var properties =
                    _remoteRepository.GetProductProperties(pageIndex++, PageSize, lastUpdateDateTime).ToList();

                if (properties.Count == 0)
                {
                    Log.ErrorFormat("没有可同步的信息,pageIndex:{0},pageSize:{1},lastUpdateDatetime:{2}", pageIndex, PageSize, lastUpdateDateTime);
                    break;
                }
                foreach (var p in properties)
                {
                    using (var db = new YintaiHZhouContext())
                    {
                        var product = db.Products.FirstOrDefault(i => i.CreatedDate > benchDate &&
                                                                      i.SkuCode.ToUpper() == p.ProductCode.ToUpper());
                        if (product == null)
                        {
                            Log.ErrorFormat("属性对应商品未同步，商品款号:({0})", p.ProductCode);
                            continue;
                        }
                        var property =
                            db.ProductProperties.FirstOrDefault(
                                x => x.ProductId == product.Id && x.ChannelPropertyId == p.PropertyId);
                        if (property == null)
                        {
                            property = db.ProductProperties.Add(new ProductProperty()
                            {
                                ProductId = product.Id,
                                ChannelPropertyId = p.PropertyId,
                                IsColor = false,
                                IsSize = false,
                                PropertyDesc = p.PropertyName,
                                SortOrder = 0,
                                Status = 1,
                                UpdateDate = DateTime.Now,
                                UpdateUser = SystemDefine.SystemUser
                            });
                        }
                        else
                        {
                            property.PropertyDesc = p.PropertyName;
                            property.UpdateDate = DateTime.Now;
                        }
                        db.SaveChanges();

                        var channelValueId = Convert.ToInt32(p.PropertyValueId);

                        var pValue =
                            db.ProductPropertyValues.FirstOrDefault(
                                x =>
                                    x.PropertyId == property.Id &&
                                    x.ChannelValueId == channelValueId);
                        if (pValue == null)
                        {
                            db.ProductPropertyValues.Add(new ProductPropertyValue()
                            {
                                ChannelValueId = channelValueId,
                                CreateDate = DateTime.Now,
                                PropertyId = property.Id,
                                Status = 1,
                                UpdateDate = DateTime.Now,
                                ValueDesc = p.ValueName,
                            });
                        }
                        else
                        {
                            pValue.ValueDesc = p.ValueName;
                            pValue.UpdateDate = DateTime.Now;
                        }
                        db.SaveChanges();
                    }
                }
                Thread.Sleep(500);
            }
        }

    }
}
