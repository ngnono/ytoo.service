using com.intime.fashion.common.message;
using com.intime.fashion.service.search;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;

namespace com.intime.fashion.service.messages.Message
{
   public class ComboHandler:MessageHandler
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
