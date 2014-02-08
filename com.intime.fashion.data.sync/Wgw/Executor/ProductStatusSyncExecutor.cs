using System.Collections.Generic;
using System.Linq.Expressions;
using com.intime.fashion.data.sync.Wgw.Request.Item;
using System;
using System.Linq;
using com.intime.jobscheduler.Job.Wgw;
using Yintai.Hangzhou.Data.Models;
using ILog = Common.Logging.ILog;

namespace com.intime.fashion.data.sync.Wgw.Executor
{
    public class ProductStatusSyncExecutor : ExecutorBase
    {
        int _succeedCount = 0;
        int _failedCount = 0;
        int _totalCount = 0;
        public ProductStatusSyncExecutor(DateTime benchTime, ILog logger)
            : base(benchTime, logger)
        {
        }

        protected override int SucceedCount
        {
            get { return _succeedCount; }
        }

        protected override int FailedCount
        {
            get { return _failedCount; }
        }

        protected override int TotalCount
        {
            get { return _totalCount; }
        }

        private void DoQuery(Expression<Func<ProductEntity, bool>> whereCondition,
            Action<IQueryable<ProductEntity>> callback)
        {
            using (var db = DbContextHelper.GetDbContext())
            {
                var items = db.Set<ProductEntity>()
                    .Where(
                        p =>
                            p.UpdatedDate >= BenchTime)
                    .Join(db.Set<Map4Product>().Where(m => m.Channel == ConstValue.WGW_CHANNEL_NAME), p => p.Id,
                        m => m.ProductId, (p, m) => new { p, m }).Where(x => x.p.Status != x.m.Status || (!x.p.Is4Sale.HasValue || !x.p.Is4Sale.Value) && x.m.Status == 1).Select(x=>x.p);
   
                if (whereCondition != null)
                    items = items.Where(whereCondition);
                if (callback != null)
                    callback(items);
            }
        }

        protected override void ExecuteCore(dynamic parameter = null)
        {
            int pageSize = parameter ?? 20;
            
            int cursor = 0;
            Expression<Func<ProductEntity, bool>> whereCondition = b=>b.UpdatedDate > BenchTime;
            
            DoQuery(whereCondition, items => _totalCount = items.Count());
            while (cursor < _totalCount)
            {
                List<ProductEntity> oneTimeList = null;
                DoQuery(whereCondition,
                    items =>
                        oneTimeList =
                            items.OrderBy(i => i.Id)
                                //.Where(o => o.IsHasImage && o.Is4Sale.HasValue && o.Is4Sale.Value && o.Status == 1)
                                .Skip(cursor)
                                .Take(pageSize)
                                .ToList());
                
                foreach (var item in oneTimeList)
                {
                    try
                    {
                        SyncOne(item);
                        _succeedCount += 1;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
                cursor += pageSize;
            }
        }

        private void SyncOne(ProductEntity item)
        {
            int productId = item.Id;
            using (var db = DbContextHelper.GetDbContext())
            {
                var map =
                    db.Map4Products.FirstOrDefault(
                        m => m.ProductId == productId && m.Channel == ConstValue.WGW_CHANNEL_NAME);
                if (map == null)
                {
                    throw new WgwSyncException(string.Format("商品未上传至微购物{0} ID:({1})",item.Name,item.Id));
                }
                var idList = new List<string>() {map.ChannelProductId};
                ISyncRequest reqeust = null;
                if (item.Is4Sale.HasValue && item.Is4Sale.Value && item.IsHasImage && item.Status == 1)
                {

                        reqeust = new UpItemRequest(idList);
                        var rsp = Client.Execute<dynamic>(reqeust);
                        if (rsp.errorCode == 0)
                        {
                            _succeedCount += 1;
                            map.Status = 1;
                            map.UpdateDate = DateTime.Now;
                            db.SaveChanges();
                            Logger.Error(string.Format("成功上架商品{0}", item.Name));
                        }
                        else
                        {
                            _failedCount += 1;
                            Logger.Error(string.Format("上架商品失败{0}", item.Name));
                        }
           
                }
                else
                {
                    reqeust = new DownItemRequest(idList);
                    var rsp = Client.Execute<dynamic>(reqeust);
                    if (rsp.errorCode == 0)
                    {
                        _succeedCount += 1;
                        map.Status = 0;
                        map.UpdateDate = DateTime.Now;
                        db.SaveChanges();
                        Logger.Error(string.Format("成功下架商品{0}", item.Name));
                    }
                    else
                    {
                        _failedCount += 1;
                        Logger.Error(string.Format("下架商品失败{0}", item.Name));
                    }
                }
            }
        }
    }
}
