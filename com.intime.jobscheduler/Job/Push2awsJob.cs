using Common.Logging;
using Nest;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.jobscheduler.Job
{
    [DisallowConcurrentExecution]
    public class Push2awsJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(typeof(Push2awsJob));
            JobDataMap data = context.JobDetail.JobDataMap;
            var esUrl = data.GetString("eshost");
            var esIndex = data.GetString("defaultindex");
            var benchDate = data.ContainsKey("benchdate") ? data.GetDateTimeValue("benchdate") : DateTime.Today.AddDays(-1);
            benchDate = benchDate<new DateTime(2013,1,1)?DateTime.Today.AddDays(-1):benchDate;
            var client = new ElasticClient(new ConnectionSettings(esUrl,9200)
                                    .SetDefaultIndex(esIndex)
                                    .SetMaximumAsyncConnections(10));
            ConnectionStatus connectionStatus;
            if (!client.TryConnect(out connectionStatus))
            {
                log.Fatal(string.Format("Could not connect to {0}:\r\n{1}",
                    esUrl, connectionStatus.Error.OriginalException.Message));
                return;
            }
            IndexProds(client, benchDate);
            IndexPros(client,benchDate);
  
        }

        private void IndexPros(ElasticClient client,DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(typeof(Push2awsJob));
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Promotions
                            join s in db.Stores on p.Store_Id equals s.Id
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            let resource = (from r in db.Resources
                                           where r.SourceId == p.Id
                                           && r.SourceType == 2
                                           select new ESResource() { 
                                             Domain = r.Domain,
                                             Name = r.Name,
                                             SortOrder = r.SortOrder,
                                             IsDefault = r.IsDefault,
                                             Type= r.Type
                                           })
                            select new ESPromotion()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                CreatedDate = p.CreatedDate,
                                StartDate = p.StartDate,
                                EndDate = p.EndDate,
                                FavoriteCount = p.FavoriteCount,
                                IsTop = p.IsTop,
                                Status = p.Status,
                                Store = new ESStore()
                                {
                                    Name = s.Name,
                                    Description = s.Description,
                                    Address = s.Location,
                                    Location = new Location{
                                         Lon = s.Longitude,
                                         Lat = s.Latitude
                                    },
                                    GpsAlt = s.GpsAlt,
                                    GpsLat = s.GpsLat,
                                    GpsLng = s.GpsLng,
                                    Tel = s.Tel
                                },
                                Resource = resource
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESPromotion>();
                while (cursor < totalCount)
                {
                    var result = client.IndexMany(prods.OrderByDescending(p => p.Id).Skip(cursor).Take(size));
                    if (!result.IsValid)
                    {
                        foreach(var item in result.Items)
                        {
                            if (item.OK)
                                successCount++;
                            else
                                log.Info(string.Format("id index failed:{0}",item.Id));
                        }
                    } else
                        successCount+=result.Items.Count();

                    cursor += size;
                }

            }
            sw.Stop();
            log.Info(string.Format("{0} promotions in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexProds(ElasticClient client,DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(typeof(Push2awsJob));
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Products
                            join s in db.Stores on p.Store_Id equals s.Id
                            join b in db.Brands on p.Brand_Id equals b.Id
                            join t in db.Tags on p.Tag_Id equals t.Id
                            where p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate
                            let resource = (from r in db.Resources
                                           where r.SourceId == p.Id
                                           && r.SourceType == 1
                                           select new ESResource()
                                           {
                                               Domain = r.Domain,
                                               Name = r.Name,
                                               SortOrder = r.SortOrder,
                                               IsDefault = r.IsDefault,
                                               Type = r.Type
                                           })
                            select new ESProduct()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                CreatedDate = p.CreatedDate,
                                Price = p.Price,
                                RecommendedReason = p.RecommendedReason,
                                Status = p.Status,
                                Tag = new ESTag()
                                {
                                    Name = t.Name,
                                    Description = t.Description
                                },
                                Store = new ESStore()
                                {
                                    Name = s.Name,
                                    Description = s.Description,
                                    Address = s.Location,
                                    Location = new Location{
                                         Lon = s.Longitude,
                                         Lat = s.Latitude
                                    },
                                    GpsAlt = s.GpsAlt,
                                    GpsLat = s.GpsLat,
                                    GpsLng = s.GpsLng,
                                    Tel = s.Tel
                                },
                                Brand = new ESBrand()
                                {
                                    Name = b.Name,
                                    Description = b.Description,
                                    EngName = b.EnglishName
                                },
                                Resource = resource
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESProduct>();
                while (cursor < totalCount)
                {
                    var result = client.IndexMany(prods.OrderByDescending(p => p.Id).Skip(cursor).Take(size));
                    if (!result.IsValid)
                    {
                        foreach (var item in result.Items)
                        {
                            if (item.OK)
                                successCount++;
                            else
                                log.Info(string.Format("id index failed:{0}", item.Id));
                        }
                    }
                    else
                        successCount += result.Items.Count();

                    cursor += size;
                }
            }
            sw.Stop();
            log.Info(string.Format("{0} products in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }
    }
}
