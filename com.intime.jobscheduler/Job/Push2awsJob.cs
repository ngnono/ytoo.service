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
            ILog log = LogManager.GetLogger(this.GetType());
            JobDataMap data = context.JobDetail.JobDataMap;
            var esUrl = data.GetString("eshost");
            var esIndex = data.GetString("defaultindex");
            var benchDate = BenchDate(context) ;
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
            IndexBrand(client, benchDate);
            IndexStore(client, benchDate);
            IndexTag(client, benchDate);
            IndexProds(client, benchDate);
            IndexPros(client,benchDate);
            IndexSpecialTopic(client, benchDate);
  
        }

        private void IndexTag(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Tags
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            select new ESTag()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Status = p.Status,
                                SortOrder = p.SortOrder

                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESTag>();
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
            log.Info(string.Format("{0} tags in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexStore(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from s in db.Stores
                            where (s.CreatedDate >= benchDate || s.UpdatedDate >= benchDate)
                            select new ESStore()
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Description = s.Description,
                                Address = s.Location,
                                Location = new Location
                                {
                                    Lon = s.Longitude,
                                    Lat = s.Latitude
                                },
                                GpsAlt = s.GpsAlt,
                                GpsLat = s.GpsLat,
                                GpsLng = s.GpsLng,
                                Tel = s.Tel

                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESStore>();
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
            log.Info(string.Format("{0} stores in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexBrand(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Brands
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            select new ESBrand()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Status = p.Status
            
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESBrand>();
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
            log.Info(string.Format("{0} brands in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

     
        private void IndexSpecialTopic(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.SpecialTopics
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            let resource = (from r in db.Resources
                                            where r.SourceId == p.Id
                                            && r.SourceType == 9
                                            select new ESResource()
                                            {
                                                Domain = r.Domain,
                                                Name = r.Name,
                                                SortOrder = r.SortOrder,
                                                IsDefault = r.IsDefault,
                                                Type = r.Type,
                                                Width = r.Width,
                                                Height = r.Height
                                            })
                            select new ESSpecialTopic()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Status = p.Status,
                                CreatedDate = p.CreatedDate,
                                CreateUser = p.CreatedUser,
                                Resource = resource
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESSpecialTopic>();
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
            log.Info(string.Format("{0} special topics in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }
        protected virtual DateTime BenchDate(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
             return data.ContainsKey("benchdate") ? data.GetDateTimeValue("benchdate") : DateTime.Today.AddDays(-1);
        }
        private void IndexPros(ElasticClient client,DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
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
                                             Type = r.Type,
                                             Width = r.Width,
                                             Height = r.Height
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
                                CreateUserId = p.CreatedUser,
                                Store = new ESStore()
                                {
                                    Id = s.Id,
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
            ILog log = LogManager.GetLogger(this.GetType());
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
                                                Type = r.Type,
                                                Width = r.Width,
                                                Height = r.Height
                                            })
                            let specials = from psp in db.SpecialTopicProductRelations
                                            where psp.Product_Id == p.Id 
                                            join sp in db.SpecialTopics on psp.SpecialTopic_Id equals sp.Id
                                            select new ESSpecialTopic
                                            {
                                                Id = sp.Id,
                                                Name = sp.Name,
                                                Description = sp.Description
                                            }
                            let promotions = from ppr in db.Promotion2Product
                                             where ppr.ProdId == p.Id
                                             join pro in db.Promotions on ppr.ProId equals pro.Id
                                             select new ESPromotion { 
                                                Id = pro.Id,
                                                Name = pro.Name,
                                                Description = pro.Description,
                                                CreatedDate = p.CreatedDate,
                                                StartDate = pro.StartDate,
                                                EndDate = pro.EndDate,
                                                Status = pro.Status
                                             }
                            select new ESProduct()
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                CreatedDate = p.CreatedDate,
                                Price = p.Price,
                                RecommendedReason = p.RecommendedReason,
                                Status = p.Status,
                                CreateUserId = p.CreatedUser,
                                SortOrder = p.SortOrder,
                                Tag = new ESTag()
                                {
                                    Id = t.Id,
                                    Name = t.Name,
                                    Description = t.Description
                                },
                                Store = new ESStore()
                                {
                                    Id =  s.Id,
                                    Name = s.Name,
                                    Description = s.Description,
                                    Address = s.Location,
                                    Location = new Location
                                    {
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
                                    Id = b.Id,
                                    Name = b.Name,
                                    Description = b.Description,
                                    EngName = b.EnglishName
                                },
                                Resource = resource,
                                SpecialTopic = specials,
                                Promotion = promotions
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
