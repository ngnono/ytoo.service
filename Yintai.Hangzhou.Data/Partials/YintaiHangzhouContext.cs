using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Framework.ServiceLocation;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class YintaiHangzhouContext
    {

        public override int SaveChanges()
        {

            List<dynamic> syncing = ParseEntitySynced().ToList();
            List<dynamic> accountTriggering = ParseEntityAccountTriggered().ToList();
            List<dynamic> notifying = ParseEntityNotified().ToList();

            var result = base.SaveChanges();

            if (result > 0 && syncing.Count() > 0)
                DoSyncing(syncing);
            if (result > 0 && accountTriggering.Count > 0)
                DoTriggering(accountTriggering);
            if (result > 0 && notifying.Count > 0)
                DoNotify(notifying);

            return result;
        }

        private void DoNotify(List<dynamic> notifying)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var tr in notifying)
                    tr.Notifying(tr.Id);
            });
        }

        private IEnumerable<dynamic> ParseEntityNotified()
        {

            foreach (var entry in this.ObjectContext.ObjectStateManager
                                  .GetObjectStateEntries(EntityState.Added))
            {

                if (entry.Entity is INotifyable)
                {
                    var syncableEntity = entry.Entity as INotifyable;
                    Action<int> composer = (id) => syncableEntity.Notify(id);
                    yield return new
                    {
                        Id = syncableEntity.NotifyId,
                        Notifying = composer
                    };
                }
            }
        }

        private void DoTriggering(List<dynamic> accountTriggering)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var tr in accountTriggering)
                    tr.Syncing(tr.UserId);
            });
        }

        private IEnumerable<dynamic> ParseEntityAccountTriggered()
        {
            foreach (var entry in this.ObjectContext.ObjectStateManager
                                   .GetObjectStateEntries(EntityState.Added | EntityState.Modified |EntityState.Deleted))
            {
                if (entry.Entity is IAccountable)
                {
                    var syncableEntity = entry.Entity as IAccountable;
                    Action<int> composer = (user) => syncableEntity.AccountSyncing(user);
                    yield return new
                    {
                        UserId = syncableEntity.AccountUserId,
                        Syncing = composer
                    };
                }
            }
        }

        private void DoSyncing(IEnumerable<dynamic> syncing)
        {
            foreach (var sync in syncing)
            {
                AwsHelper.SendMessage(sync.TypeName
                    , sync.Composer);
            }
        }

        private IEnumerable<dynamic> ParseEntitySynced()
        {
            foreach (var entry in this.ObjectContext.ObjectStateManager
                                    .GetObjectStateEntries(EntityState.Added))
            {
                if (entry.Entity is ISyncable)
                {
                    var syncableEntity = entry.Entity as ISyncable;
                    Func<object> composer = () => { return syncableEntity.Composing(); };
                    yield return new
                    {
                        TypeName = syncableEntity.TypeName
                        ,
                        Id = syncableEntity.SyncId
                        ,
                        Operation = entry.State
                        ,
                        Composer = composer
                    };
                }
            }

        }
    }
}
