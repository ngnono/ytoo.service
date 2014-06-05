﻿using com.intime.fashion.common.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Service.Logic.Search;

namespace com.intime.jobscheduler.Message
{
    class ProductHandler:MessageHandler
    {
         private ESServiceBase _esService = null;
         public ProductHandler()
        {
            _esService = SearchLogic.GetService(Yintai.Hangzhou.Model.Enums.SourceType.Product);
        }
        public override int SourceType
        {
            get { return (int)MessageSourceType.Product; }
        }

        public override int ActionType
        {
            get { return (int)(MessageAction.CreateEntity | MessageAction.UpdateEntity); }
        }

        public override bool Work(BaseMessage message)
        {
            using (var slt = new ScopedLifetimeDbContextManager())
            {
                return _esService.IndexSingle(message.EntityId);
            }
        }
    }
}
