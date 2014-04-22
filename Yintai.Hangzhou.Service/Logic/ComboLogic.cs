
using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Service.Logic
{
    public static class ComboLogic
    {
        public static bool IfCanOnline(int userId)
        {
            var onlineCount = Context.Set<IMS_AssociateItemsEntity>().Where(ia=>ia.Status==(int)DataStatus.Normal && ia.ItemType==(int)ComboType.Product).
                         Join(Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == userId), o => o.AssociateId, i => i.Id,
                                (o, i) => o).Count();
            return onlineCount < ConfigManager.MAX_COMBO_ONLINE;
        }
        private static DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }
    }
}
