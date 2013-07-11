using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class PMessageEntity: INotifyable
    {

        public int NotifyId
        {
            get { return this.Id; }
        }

        public void Notify(int id)
        {
            int notifiedId = NotifyId;
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                db.NotificationLogs.Add(new NotificationLogEntity()
                {
                    CreateDate = DateTime.Now,
                    SourceId = notifiedId,
                    SourceType = (int)Yintai.Hangzhou.Model.Enums.SourceType.PMessage,
                    Status = (int)NotificationStatus.Default

                });
                db.SaveChanges();

            }
        }
    }
}
