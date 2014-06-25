using com.intime.fashion.common.message;
using com.intime.fashion.common.message.rabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Impl;

namespace com.intime.fashion.messagelistener
{
    class UnityBootStrapper
    {
        public static void Init()
        {
            ServiceLocator.Current.RegisterSingleton<Yintai.Architecture.Common.Logger.ILog, Yintai.Architecture.Common.Logger.Log4NetLog>();
            ServiceLocator.Current.Register<IMessageCenterProvider, MessageProvider>();
            ServiceLocator.Current.Register<IEFRepository<IMS_ComboEntity>, ComboRepository>();
        }
    }
}
