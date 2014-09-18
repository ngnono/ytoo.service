using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.intime.fashion.common.message;
using Top.Api;
using Top.Api.Request;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.service.messages.Message
{
    public class ComboUpdatedHandler : MessageHandler
    {
        private DbContext _db;
        private ITopClient _topClient;
        public ComboUpdatedHandler()
        {
            _topClient = new DefaultTopClient(InventoryUpdatedHandler.TOP_SERVERURL, InventoryUpdatedHandler.TOP_APPKEY, InventoryUpdatedHandler.TOP_APPSECRET);
            _db = ServiceLocator.Current.Resolve<DbContext>();
        }

        public override int SourceType
        {
            get { return (int)MessageSourceType.Combo; }
        }

        public override int ActionType
        {
            get
            {
                return (int)(MessageAction.UpdateEntity | MessageAction.DeleteEntity);
            }
        }

        public override bool Work(BaseMessage message)
        {
            int comboId = message.EntityId;

            var combo = _db.Set<IMS_ComboEntity>().FirstOrDefault(x => x.Id == comboId);
            if (combo == null)
            {
                return false;
            }

            var mappedProducts =
                    _db.Set<IMS_Combo2ProductEntity>()
                        .Where(x => x.ComboId == message.EntityId)
                        .Join(_db.Set<Map4ProductEntity>().Where(m => m.Channel == "tmall"), c => c.ProductId,
                            m => m.ProductId, (c, m) => m);

            bool allSucceed = true;
            //下架删除搭配的商品或下架的搭配下的商品
            if (combo.Status == (int)DataStatus.Default || combo.Status == (int)DataStatus.Deleted)
            {

                foreach (var entity in mappedProducts)
                {
                    var req = new ItemUpdateDelistingRequest
                    {
                        NumIid = Convert.ToInt64(entity.ChannelProductId)
                    };
                    var rsp = _topClient.Execute(req);
                    if (rsp.IsError)
                    {
                        allSucceed = false;
                    }
                }
                return allSucceed;
            }

            // todo 上架，上架分两步，上架并同时更新库存
            foreach (var entity in mappedProducts)
            {
                var stocks =
                    _db.Set<Map4InventoryEntity>()
                        .Where(x => x.ProductId == entity.ProductId && x.Channel == "tmall")
                        .Join(_db.Set<InventoryEntity>(), m => m.InventoryId, i => i.Id,
                            (m, i) => new {Map4Inventory = m, Inventory = i});
                
                //先更新库存，再上线
                if (Enumerable.Any(stocks, stock => !InventoryUpdatedHandler.SyncInventory(stock.Inventory.Id, _db, _topClient)))
                {
                    continue;
                }          

                var req = new ItemUpdateListingRequest
                {
                    NumIid = Convert.ToInt64(entity.ChannelProductId),
                    Num = stocks.Count(x => x.Inventory.Amount > 0)
                };
                if (_topClient.Execute(req).IsError)
                {
                    allSucceed = false;
                }
            }

            return allSucceed;
        }
    }
}
