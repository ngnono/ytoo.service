using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using com.intime.fashion.data.sync.Wgw;
using com.intime.fashion.data.sync.Wgw.Request.Item;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.data.sync.runner.Runner
{
    internal class DownItemRunner:Runner
    {
        private void DoQuery(Expression<Func<MappedProductBackup, bool>> whereCondition, Action<IQueryable<MappedProductBackup>> callback)
        {
            using (var context = DbContextHelper.GetDbContext())
            {
                var linq = context.Set<MappedProductBackup>().AsQueryable();
                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }
        protected override void Do()
        {
            int count = 0;
            DoQuery(null,maps => count = maps.Count());
            int cursor = 0;
            const int pageSize = 200;
            var request = new DownItemRequest();
            while (cursor < count)
            {
                IEnumerable<string> maps = null;
                DoQuery(null,items => maps = items.OrderBy(t=>t.Id).Skip(cursor).Take(pageSize).Select(t=>t.ChannelProductId).ToList());
                request.Put(ParamName.Param_ItemList,string.Join("|",maps));
                request.Remove("sign");
                var rsp = Client.Execute<dynamic>(request);
                if (rsp.errorCode == 0)
                {
                    Logger.Info(string.Format("共计下架商品（{0}）", maps.Count()));
                }
                else if (rsp.errorCode == 40005) //部分失败
                {
                    string failList = rsp.failList;
                    Logger.Error(failList);
                }
                else
                {
                    Logger.Error(rsp.errorMessage);
                }
                cursor += pageSize;
            }
        }
    }
}