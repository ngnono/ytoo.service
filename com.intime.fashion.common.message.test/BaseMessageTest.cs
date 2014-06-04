using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.intime.fashion.common.message.test.fixture;
using Newtonsoft.Json;

namespace com.intime.fashion.common.message.test
{
    [TestClass]
    public class BaseMessageTest
    {
        [TestMethod]
        public void TestBaseMessageSerialize()
        {
            var rawMessage = BaseMessageData.GetRawMessage();
            var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(rawMessage);

            Assert.IsNotNull(baseMessage);
            Assert.AreEqual(baseMessage.ActionType, (int)MessageAction.UpdateEntity);
        }
    }
}
