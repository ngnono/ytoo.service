using com.intime.fashion.common.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Service.Logic.Search;

namespace com.intime.jobscheduler.Message
{
    class InventoryHandler:MessageHandler
    {
       private ESServiceBase _esService = null;
         public InventoryHandler()
        {
            _esService = SearchLogic.GetService(Yintai.Hangzhou.Model.Enums.SourceType.Product);
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
            var productId = 0;
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var inventoryEntity = db.Set<InventoryEntity>().Find(message.EntityId);
                if (inventoryEntity == null)
                    return false;
                productId = inventoryEntity.ProductId;
            }
            using (var slt = new ScopedLifetimeDbContextManager())
            {
                return _esService.IndexSingle(productId);
            }
        }
    }
}
