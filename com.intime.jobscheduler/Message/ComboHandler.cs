﻿using com.intime.fashion.common.message;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Service.Logic.Search;

namespace com.intime.jobscheduler.Message
{
    class ComboHandler:MessageHandler
    {
        private ESServiceBase _esService = null;
        public ComboHandler()
        {
            _esService = SearchLogic.GetService(Yintai.Hangzhou.Model.Enums.SourceType.Combo);
        }
        public override int SourceType
        {
            get { return (int)MessageSourceType.Combo; }
        }

        public override int ActionType
        {
            get
            {
                return (int)(MessageAction.CreateEntity |
                      MessageAction.UpdateEntity |
                      MessageAction.DeleteEntity);
            }
        }
        public override bool Work(BaseMessage message)
        {
            var comboId = message.EntityId;
           return _esService.IndexSingle(comboId);
            
        }
    }
}
