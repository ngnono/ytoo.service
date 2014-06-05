using com.intime.fashion.common.message;
using com.intime.fashion.common.message.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class InventoryRepository : RepositoryBase<InventoryEntity, int>,IInventoryRepository
    {
        public override void Update(InventoryEntity entity)
        {
            base.Update(entity);

            //step5: notify message
            var messageProvider = ServiceLocator.Current.Resolve<IMessageCenterProvider>();
            messageProvider.GetSender().SendMessageReliable(new UpdateMessage()
            {
                SourceType = (int)MessageSourceType.Inventory,
                EntityId = entity.Id
            });
        }

      
    }
}
