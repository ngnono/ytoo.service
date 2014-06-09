using com.intime.fashion.common.message;
using com.intime.fashion.common.message.Messages;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class ComboRepository : EFRepository<IMS_ComboEntity>
    {
        public ComboRepository() : base(ServiceLocator.Current.Resolve<DbContext>()) { }
        public override void Update(IMS_ComboEntity entityToUpdate)
        {
            base.Update(entityToUpdate);

            Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler((o, e) =>
            {
                if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                {
                    var messageProvider = ServiceLocator.Current.Resolve<IMessageCenterProvider>();
                    messageProvider.GetSender().SendMessageReliable(new UpdateMessage()
                    {
                        SourceType = (int)MessageSourceType.Combo,
                        EntityId = entityToUpdate.Id
                    });
                }
                
            });
        }

        public override Yintai.Hangzhou.Data.Models.IMS_ComboEntity Insert(Yintai.Hangzhou.Data.Models.IMS_ComboEntity entity)
        {
            var newEntity = base.Insert(entity);
            Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler((o, e) =>
            {
                if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                {
                    var messageProvider = ServiceLocator.Current.Resolve<IMessageCenterProvider>();
                    messageProvider.GetSender().SendMessageReliable(new CreateMessage()
                    {
                        SourceType = (int)MessageSourceType.Combo,
                        EntityId = newEntity.Id
                    });
                }

            });
            return newEntity;
        }
    }
}
