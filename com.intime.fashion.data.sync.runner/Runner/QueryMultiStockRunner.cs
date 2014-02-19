using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.intime.fashion.data.sync.Wgw.Executor;
using com.intime.fashion.data.sync.Wgw.Request.Item;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class QueryMultiStockRunner:Runner
    {
        //private string _itemId;
        //public QueryMultiStockRunner(string itemId)
        //{
        //    this._itemId = itemId;
        //}

        protected override void Do()
        {
            //var getItemMultiStockRequest = new GetItemMultiStockRequest(_itemId);
            //var rsp = Client.Execute<dynamic>(getItemMultiStockRequest);
            var executor = new GetItemMultiStockExecutor(DateTime.Now.AddHours(-1),Logger);
            executor.Execute();
            //Console.WriteLine(rsp);
        }
    }
}
