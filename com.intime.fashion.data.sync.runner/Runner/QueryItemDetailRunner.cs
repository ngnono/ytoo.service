using System;
using com.intime.fashion.data.sync.Wgw.Request.Item;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class QueryItemDetailRunner:Runner
    {
        private string _itemId;

        public QueryItemDetailRunner(string itemId)
        {
            this._itemId = itemId;
        }
        protected override void Do()
        {
            var request = new QueryItemDetailRequest();
            request.Put("itemid", _itemId);
            var rsp = this.Client.Execute<dynamic>(request);
            Console.WriteLine(rsp);
        }
    }
}
