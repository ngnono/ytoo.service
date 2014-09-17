using com.intime.fashion.common.message;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Top.Api;
using Top.Api.Request;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;


namespace com.intime.fashion.service.messages.Message
{
    public class InventoryUpdatedHandler : MessageHandler
    {
        private DbContext _context;
        private ITopClient _topClient;
        internal static string TOP_SERVERURL = ConfigurationManager.AppSettings["TOP_SERVERURL"];
        internal static string TOP_APPKEY = ConfigurationManager.AppSettings["TOP_APPKEY"];
        internal static string TOP_APPSECRET = ConfigurationManager.AppSettings["TOP_APPSECRET"];
        public InventoryUpdatedHandler()
        {
            _context = ServiceLocator.Current.Resolve<DbContext>();
            _topClient = new DefaultTopClient(TOP_SERVERURL, TOP_APPKEY, TOP_APPSECRET);
        }
        public override int SourceType
        {
            get { return (int)MessageSourceType.Inventory; }
        }

        public override int ActionType
        {
            get { return (int)(MessageAction.CreateEntity | MessageAction.UpdateEntity); }
        }

        public override bool Work(BaseMessage message)
        {
            return SyncInventory(message.EntityId, _context, _topClient);
        }

        internal static bool SyncInventory(int inventoryId, DbContext db, ITopClient topClient)
        {
            var stockInfo =
                 db.Set<InventoryEntity>()
                     .Where(x => x.Id == inventoryId)
                     .Join(
                         db.Set<Map4InventoryEntity>()
                             .Where(m => m.Channel == "tmall" && m.InventoryId == inventoryId), i => i.Id,
                         m => m.InventoryId, (i, m) => new { Inventory = i, Map4Inventory = m })
                     .FirstOrDefault();
            if (stockInfo == null)
            {
                return false;
            }
            var req = new ItemQuantityUpdateRequest
            {
                OuterId = stockInfo.Inventory.Id.ToString(),
                Quantity = stockInfo.Inventory.Amount,
                NumIid = Convert.ToInt64(stockInfo.Map4Inventory.itemId),
                SkuId = stockInfo.Map4Inventory.skuId
            };

            return topClient.Execute(req).IsError;
        }
    }
}
