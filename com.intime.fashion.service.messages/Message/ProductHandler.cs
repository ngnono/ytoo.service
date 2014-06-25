﻿using com.intime.fashion.common.message;
using com.intime.fashion.common.message.Messages;
using com.intime.fashion.service.search;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;

namespace com.intime.fashion.service.messages.Message
{
    public class ProductHandler : MessageHandler
    {
        private ESServiceBase _esService = null;
        private ComboService _comboService = null;
        public ProductHandler()
        {
            _esService = SearchLogic.GetService(IndexSourceType.Product);
            _comboService = new ComboService();
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
            if (message is UpdateMessage)
            {
                _comboService.RefreshPrice(message.EntityId);
               
            }
            return _esService.IndexSingle(message.EntityId);
        }
    }
}
