using com.intime.fashion.data.erp.Models;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yintai.Hangzhou.Contract.Images;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Manager;

namespace com.intime.jobscheduler.Job.Erp
{
    [DisallowConcurrentExecution]
    class ProductPicSyncJob:IJob
    {
        private void DoQuery(Expression<Func<PRO_PICTURE, bool>> whereCondition, Action<IQueryable<PRO_PICTURE>> callback)
        {
            using (var context = new ErpContext())
            {
                var linq = context.PRO_PICTURE.AsQueryable();
                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }
        public void Execute(IJobExecutionContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            JobDataMap data = context.JobDetail.JobDataMap;
            var isRebuild = data.ContainsKey("isRebuild") ? data.GetBoolean("isRebuild") : false;
            var interval = data.ContainsKey("intervalOfSecs") ? data.GetInt("intervalOfSecs") : 5 * 60;
            var totalCount = 0;
            var benchTime = DateTime.Now.AddSeconds(-interval);
            Expression<Func<PRO_PICTURE, bool>> whereCondition = null;
            if (!isRebuild)
                whereCondition = b => b.OPT_UPDATE_TIME >= benchTime;

            DoQuery(whereCondition, products =>
            {
                totalCount = products.Count();
            });
            int cursor = 0;
            int successCount = 0;
            int size = 100;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (cursor < Math.Min(totalCount,10))
            {
                List<PRO_PICTURE> oneTimeList = null;
                DoQuery(whereCondition, products =>
                {
                    oneTimeList = products.OrderBy(b => b.SID).Skip(cursor).Take(size).ToList();
                });
                foreach (var product in oneTimeList)
                {
                    SyncOne(product);
                    successCount++;
                }
  
                cursor += size;
            }

            sw.Stop();
            log.Info(string.Format("total pics:{0},{1} ex pics in {2} => {3} docs/s", totalCount, successCount, sw.Elapsed, successCount / sw.Elapsed.TotalSeconds));


        }

        private string FetchRemotePic(string url)
        {
            var client = new HttpClient();
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,DateTime.Today.ToString("yyyyMMdd"));
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var path = string.Format("{0}/{1}.jpg",directory,Guid.NewGuid());
             client.GetAsync(url)
                .ContinueWith(request =>
                 {
                    var response = request.Result;
                    response.EnsureSuccessStatusCode();
                    using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                      response.Content.CopyToAsync(fileStream).Wait();
                      fileStream.Flush();
                     
                    }
                }).Wait();
            return path;
                
        }

        private void EnsureProductContext(PRO_PICTURE product)
        {
            using(var erpDb= new ErpContext())
            {
                
                using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                {
                    var colorEntity = db.Set<ProductPropertyValueEntity>().Where(ppv => ppv.ChannelValueId == product.PRO_COLOR_SID)
                                    .Join(db.Set<ProductPropertyEntity>().Join(db.Set<ProductMapEntity>().Where(pm=>pm.ChannelPId==product.PRODUCT_SID),o=>o.ProductId,i=>i.ProductId,(o,i)=>o),
                                            o=>o.PropertyId,i=>i.Id,(o,i)=>o).FirstOrDefault();
                    if (colorEntity == null)
                    {
                        var exProduct = erpDb.Set<SUPPLY_MIN_PRICE_MX>().Where(ep => ep.PRODUCT_SID == product.PRODUCT_SID && ep.PRO_COLOR_SID == product.PRO_COLOR_SID).FirstOrDefault();
                        ProductPropertySyncJob.SyncOne(exProduct.PRODUCT_SID, exProduct.PRO_STAN_SID ?? 0, exProduct.PRO_STAN_NAME, exProduct.PRO_COLOR_SID ?? 0, exProduct.PRO_COLOR);
                    }
         
                }
             
            }
        }

        private void SyncOne(PRO_PICTURE product)
        {
            EnsureProductContext(product);
            var log = LogManager.GetLogger(this.GetType());
            //download remote picture
            string exPicDomain = ConfigurationManager.AppSettings["EXPIC_DOMAIN"];
            var filePath = FetchRemotePic(string.Format("{0}/{1}",exPicDomain.TrimEnd('/'),Path.Combine(product.PRO_PICT_DIR,product.PRO_PICT_NAME)));
            //resize pics
            var file = new FileInfo(filePath);
            FileInfor uploadFile;
            var uploadResult = FileUploadServiceManager.UploadFile(file, "product", out uploadFile, string.Empty);
            if (uploadResult != FileMessage.Success)
            {
                log.Error(string.Format("upload file error:{0}", filePath));
                 File.Delete(filePath);
                return;
            }
            
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var existPic = db.Set<ResourceEntity>().Where(r => r.ChannelPicId == product.SID).FirstOrDefault();
                if (existPic == null)
                {
                    var colorEntity = db.Set<ProductPropertyValueEntity>().Where(ppv => ppv.ChannelValueId == product.PRO_COLOR_SID)
                                        .Join(db.Set<ProductPropertyEntity>().Join(db.Set<ProductMapEntity>().Where(pm => pm.ChannelPId == product.PRODUCT_SID), o => o.ProductId, i => i.ProductId, (o, i) => o),
                                                o => o.PropertyId, i => i.Id, (o, i) => o).FirstOrDefault();
                    var existProduct = db.Set<ProductEntity>().Join(db.Set<ProductMapEntity>().Where(ep => ep.ChannelPId == product.PRODUCT_SID), o => o.Id, i => i.ProductId, (o, i) => o).FirstOrDefault();
                    db.Resources.Add(new ResourceEntity()
                    {
                        ColorId = colorEntity == null ? 0 : colorEntity.Id,
                        SourceId = existProduct.Id,
                        SourceType = (int)SourceType.Product,
                        ContentSize = uploadFile.FileSize,
                        CreatedDate = DateTime.Now,
                        CreatedUser = 0,
                        Domain = string.Empty,
                        ExtName = uploadFile.FileExtName,
                        Height = uploadFile.Height,
                        IsDefault = product.PICTURE_MAST_BIT == 1 ? true : false,
                        UpdatedDate = product.OPT_UPDATE_TIME ?? DateTime.Now,
                        Name = uploadFile.FileName,
                        Status = 1,
                        SortOrder = (int)product.PRO_PICT_ORDER,
                        Size = string.Format("{0}x{1}", uploadFile.Width, uploadFile.Height),
                        Type = (int)uploadFile.ResourceType,
                        Width = uploadFile.Width,
                        ChannelPicId = (int)product.SID
                    });
                    existProduct.IsHasImage = true;
                    existProduct.UpdatedDate = product.OPT_UPDATE_TIME ?? DateTime.Now;
                    db.SaveChanges();
                }
            }

            File.Delete(filePath);
        }
    }
}
