using System;
using com.intime.fashion.data.sync.Wgw.Request.Item;
using com.intime.fashion.data.sync.Wgw.Response.Processor;

namespace com.intime.fashion.data.sync.runner.Runner
{
    public class LoadOnlineItemsRunner:Runner
    {
        protected  override void Do()
        {
            int index = 0;
            var request = new QueryItemListRequest();
            request.Put("startIndex", index++);
            request.Put("pageSize", 100);
            request.Put("orderType", "7");
            var rsp = Client.Execute<dynamic>(request);
            int cursor = 1;

            if (rsp.errorCode == 0)
            {
                int totalNum = rsp.totalNum;
                var ps = ProcessorFactory.CreateProcessor<QueryItemListResponseProcessor>();
                Logger.Info(ps.Process(rsp, true) ? "load successfully" : ps.ErrorMessage);
                cursor += 100;
                
                while (cursor < totalNum)
                {
                    request.Remove("sign");
                    request.Put("startIndex",index ++);
                    rsp = Client.Execute<dynamic>(request);
                    ps = ProcessorFactory.CreateProcessor<QueryItemListResponseProcessor>();
                    Logger.Info(ps.Process(rsp, true) ? "load successfully" : ps.ErrorMessage);
                    cursor += 100;
                }
            }
            else
            {
                Logger.Error(rsp.errorMessage);
            }
        }
    }
}
