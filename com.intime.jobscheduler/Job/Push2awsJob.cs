using Common.Logging;
using Nest;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;

using Newtonsoft.Json;
using Yintai.Hangzhou.Model.ES;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model.Enums;
using com.intime.fashion.common;

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
            var needRebuild = data.ContainsKey("needrebuild") ? data.GetBooleanValue("needrebuild") : false;
            var benchDate = BenchDate(context) ;
            
            var client = new ElasticClient(new ConnectionSettings(new Uri(esUrl))
                                    .SetDefaultIndex(esIndex)
                                    .SetMaximumAsyncConnections(10));
            ConnectionStatus connectionStatus;
            if (!client.TryConnect(out connectionStatus))
            {
                log.Fatal(string.Format("Could not connect to {0}:\r\n{1}",
                    esUrl, connectionStatus.Error.OriginalException.Message));
                return;
            }
            if (needRebuild)
            {
                var response = client.DeleteIndex(esIndex);
                if (response.OK)
                    log.Info(string.Format("index:{0} is deleted!",esIndex));
                else
                {
                    log.Info("remove index failed");
                }
            }
            IndexBrand(client, benchDate);
            IndexHotwork(client, benchDate);
            IndexStore(client, benchDate);
            IndexTag(client, benchDate);
            IndexResource(client, benchDate);
            IndexProds(client, benchDate,null);
            IndexPros(client,benchDate,null);
            IndexBanner(client, benchDate);
            IndexSpecialTopic(client, benchDate,null);
            IndexStorePromotion(client, benchDate);
            IndexStorePromotionCode(client, benchDate);
            IndexPromotionCode(client, benchDate);
        }

        private void IndexPromotionCode(ElasticClient client, DateTime benchDate)
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

        private void IndexStorePromotionCode(ElasticClient client, DateTime benchDate)
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
                            where (r.CreateDate >= benchDate || r.UpdateDate >= benchDate)
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


        private void IndexStorePromotion(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = db.StorePromotions.Where(r=> r.CreateDate >= benchDate || r.UpdateDate >= benchDate)
                            .GroupJoin(db.PointOrderRules.Where(r=>r.Status!=(int)DataStatus.Deleted),o=>o.Id,i=>i.StorePromotionId,
                                    (o,i)=>new {S=o,R = i});

                int totalCount = prods.Count();
                client.MapFromAttributes<ESStorePromotion>();
                while (cursor < totalCount)
                {
                    var linq = from l in prods.OrderByDescending(p => p.S.Id).Skip(cursor).Take(size).ToList()
                               select new ESStorePromotion().FromEntity<ESStorePromotion>(l.S, s => { 
                                    s.ExchangeRule = JsonConvert.SerializeObject(l.R.Select(r=>new {
                                        rangefrom=r.RangeFrom,
                                        rangeto = r.RangeTo,
                                        ratio = r.Ratio
                                    }));
                               });
                    var result = client.IndexMany(linq);
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
            log.Info(string.Format("{0} store promotions in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
            
        }

        private void IndexResource(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from r in db.Resources
                            where (r.CreatedDate >= benchDate || r.UpdatedDate >= benchDate)
                            select new ESResource()
                            {
                                Id = r.Id,
                                Status= r.Status,
                                Domain = r.Domain,
                                Name = r.Name,
                                SortOrder = r.SortOrder,
                                IsDefault = r.IsDefault,
                                Type = r.Type,
                                Width = r.Width,
                                Height = r.Height,
                                SourceId = r.SourceId,
                                SourceType = r.SourceType

                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESResource>();
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
            log.Info(string.Format("{0} resources in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));
            //update related source if any affected
            if (successCount > 0 && CascadPush)
            {
                //index related products
                log.Info("index products affected by related resources ");
                IndexProds(client, null,
                    (p, db) =>
                    {
                        return p.Where(prod => (from r in db.Resources
                                                where r.SourceId == prod.Id
                                                && (r.CreatedDate >= benchDate || r.UpdatedDate >= benchDate)
                                                && r.SourceType == (int)SourceType.Product
                                                select r.Id).Any());
                    });
                //index related promotions
                log.Info("index promotions affected by related resources ");
                IndexPros(client, null,
                    (p, db) =>
                    {
                        return p.Where(prod => (from r in db.Resources
                                                where r.SourceId == prod.Id
                                                && (r.CreatedDate >= benchDate || r.UpdatedDate >= benchDate)
                                                && r.SourceType == (int)SourceType.Promotion
                                                select r.Id).Any());
                    });
                //index related specialtopics
                log.Info("index specialtopics affected by related resources ");
                IndexSpecialTopic(client, null,
                    (p, db) =>
                    {
                        return p.Where(prod => (from r in db.Resources
                                                where r.SourceId == prod.Id
                                                && (r.CreatedDate >= benchDate || r.UpdatedDate >= benchDate)
                                                && r.SourceType == (int)SourceType.SpecialTopic
                                                select r.Id).Any());
                    });
            }
        }

        private void IndexUser(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Users
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            let resource = from r in db.Resources
                                           where r.SourceId == p.Id && r.SourceType == 10
                                           select new ESResource()
                                           {
                                               Domain = r.Domain,
                                               Name = r.Name,
                                               SortOrder = r.SortOrder,
                                               IsDefault = r.IsDefault,
                                               Type = r.Type,
                                               Width = r.Width,
                                               Height = r.Height
                                           }
                            select new ESUser()
                            {
                                Id = p.Id,
                                Status = p.Status,
                                Thumnail = resource.FirstOrDefault(),
                                Nickie = p.Nickname,
                                Level = p.UserLevel

                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESUser>();
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
            log.Info(string.Format("{0} users in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexComments(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Comments
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            let resource = from r in db.Resources
                                           where r.SourceId == p.Id && r.SourceType == 10
                                           select new ESResource()
                                            {
                                                Domain = r.Domain,
                                                Name = r.Name,
                                                SortOrder = r.SortOrder,
                                                IsDefault = r.IsDefault,
                                                Type = r.Type,
                                                Width = r.Width,
                                                Height = r.Height
                                            }
                            select new ESComment()
                            {
                                Id = p.Id,
                                Status = p.Status,
                                Resource = resource,
                                ParentCommentId = p.ReplyId,
                                ParentCommentUserId = p.ReplyUser,
                                SourceId = p.SourceId,
                                SourceType = p.SourceType,
                                CreatedDate = p.CreatedDate,
                                CreateUserId = p.CreatedUser,
                                TextMsg = p.Content
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESComment>();
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
            log.Info(string.Format("{0} comments in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexHotwork(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var words = from p in db.HotWords
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                            select p;
                var prods = from p in words.ToList()
                            select new ESHotword()
                            {
                                Id = p.Id,
                                SortOrder = p.SortOrder,
                                Status = p.Status,
                                Type = p.Type,
                                Word = p.Type==1?p.Word:JsonConvert.DeserializeObject<dynamic>(p.Word).name,
                                BrandId = p.Type==1?0:JsonConvert.DeserializeObject<dynamic>(p.Word).id
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESHotword>();
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
            log.Info(string.Format("{0} hotwords in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

        }

        private void IndexBanner(ElasticClient client, DateTime benchDate)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var prods = from p in db.Banners
                            join pro in db.Promotions on p.SourceId equals pro.Id
                            where (p.CreatedDate >= benchDate || p.UpdatedDate >= benchDate)
                                && p.SourceType == 2
                            let resource = (from r in db.Resources
                                            where r.SourceId == p.Id
                                            && r.SourceType == 11
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
                            
                            select new ESBanner()
                            {
                                Id = p.Id,
                                SortOrder = p.SortOrder,
                                CreatedDate = p.CreatedDate,
                                Status = p.Status,
                                SourceType = p.SourceType,
                                Promotion = new ESPromotion()
                                {
                                    Id = pro.Id
                                  
                                },
                                Resource = resource
                            };

                int totalCount = prods.Count();
                client.MapFromAttributes<ESBanner>();
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
            log.Info(string.Format("{0} banners in {1} => {2} docs/s", successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));

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
                            let resource = (from r in db.Resources
                                            where r.SourceId == s.Id
                                            && r.SourceType == (int)SourceType.StoreLogo
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
                                Tel = s.Tel,
                                Status = s.Status,
                                Resource = resource

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
                                Status = p.Status,
                                Group = p.Group
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


        private void IndexSpecialTopic(ElasticClient client, DateTime? benchDate, Func<IQueryable<SpecialTopicEntity>, YintaiHangzhouContext, IQueryable<SpecialTopicEntity>> whereCondition)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var linq = db.SpecialTopics.AsQueryable();
                if (benchDate.HasValue)
                    linq = linq.Where(p => p.CreatedDate >= benchDate.Value || p.UpdatedDate >= benchDate.Value);
                else if (whereCondition != null)
                {
                    linq = whereCondition(linq, db);

                }
                var prods = from p in linq
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
                                Resource = resource,
                                Type = p.Type,
                                TargetValue =  p.TargetValue
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
            if (successCount > 0 && CascadPush)
            {
                //index related products
                log.Info("index products affected by related specialtopic ");
                IndexProds(client, null,
                    (p, db) =>
                    {
                        return p.Where(prod => (from pro in db.SpecialTopics
                                                from ppr in db.SpecialTopicProductRelations
                                                where ppr.SpecialTopic_Id == pro.Id
                                                && (pro.CreatedDate >= benchDate || pro.UpdatedDate >= benchDate)
                                                && ppr.Product_Id == prod.Id
                                                select ppr.Product_Id).Any());
                    });
            }
        }
        protected virtual DateTime BenchDate(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
             return data.ContainsKey("benchdate") ? data.GetDateTimeValue("benchdate") : DateTime.Today.AddDays(-1);
        }
        protected virtual bool CascadPush
        {
            get {
                return false;
            }
        }
        private void IndexPros(ElasticClient client, DateTime? benchDate, Func<IQueryable<PromotionEntity>, YintaiHangzhouContext, IQueryable<PromotionEntity>> whereCondition)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int size = 100;
            int successCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var linq = db.Promotions.AsQueryable();
                if (benchDate.HasValue)
                    linq = linq.Where(p => p.CreatedDate >= benchDate.Value || p.UpdatedDate >= benchDate.Value);
                else if (whereCondition != null)
                {
                    linq = whereCondition(linq, db);

                }
                var prods = from p in linq
                            join s in db.Stores on p.Store_Id equals s.Id
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
                                Resource = resource,
                               ShowInList = p.IsMain.HasValue?p.IsMain.Value:true,
                               PublicCode = p.PublicProCode
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
            if (successCount > 0 && CascadPush)
            { 
                //index related products
                log.Info("index products affected by related promotions ");
                IndexProds(client, null,
                    (p,db) => {
                        return p.Where(prod => (from pro in db.Promotions
                                             from ppr in db.Promotion2Product
                                             where ppr.ProId == pro.Id
                                             && (pro.CreatedDate >= benchDate || pro.UpdatedDate >= benchDate)
                                             && ppr.ProdId == prod.Id
                                             select ppr.ProdId).Any());
                    });
            }
        }

        private void IndexProds(ElasticClient client,DateTime? benchDate,Func<IQueryable<ProductEntity>,YintaiHangzhouContext,IQueryable<ProductEntity>> whereCondition)
        {
            ILog log = LogManager.GetLogger(this.GetType());
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var linq = db.Products.AsQueryable();
                if (benchDate.HasValue)
                    linq = linq.Where(p => p.CreatedDate >= benchDate.Value || p.UpdatedDate >= benchDate.Value);
                else if (whereCondition!=null)
                {
                    linq = whereCondition(linq, db);
                   
                }

                var prods = from p in linq
                            join s in db.Stores on p.Store_Id equals s.Id
                            join b in db.Brands on p.Brand_Id equals b.Id
                            join t in db.Tags on p.Tag_Id equals t.Id
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
