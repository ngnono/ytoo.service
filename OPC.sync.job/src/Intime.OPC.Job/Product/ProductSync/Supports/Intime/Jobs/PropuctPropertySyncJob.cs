using System.Linq;
using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Mapper;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
using Quartz;
using System;
using System.Threading;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs
{
    public class PropuctPropertySyncJob:IJob
    {
        private const int PageSize = 200;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        RemoteRepository _remoteRepository = new RemoteRepository(new DefaultApiClient());
        private StoreSyncProcessor _storeSyncProcessor;
        private ChannelMapper _channelMapper;
        public void Execute(IJobExecutionContext context)
        {
            this.Sync();
        }

        public void Sync()
        {
            var pageIndex = 1;


            while (true)
            {
                var properties =
                    _remoteRepository.GetProductProperties(pageIndex++, PageSize, DateTime.Now.AddYears(-2)).ToList();

                if (properties.Count == 0)
                {
                    Log.ErrorFormat("没有可同步的信息,pageIndex:{0},pageSize:{1},lastUpdateDatetime:{2}", pageIndex, PageSize);
                    break;
                }
                foreach (var channelSection in properties)
                {
                    var channelSectionId = channelSection.CounterId;
                    var channelStoreNo = channelSection.StoreNo;
                    var storeExt = _storeSyncProcessor.Sync(channelStoreNo);
                    if (storeExt == null)
                    {
                        continue;
                    }
                    using (var db = new YintaiHZhouContext())
                    {
                       

                        //    // 保存映射关系
                        //    var channelMap = new ChannelMap()
                        //    {
                        //        LocalId = newSection.Id,
                        //        ChannnelValue = channelSectionId,
                        //        MapType = ChannelMapType.SectionId
                        //    };

                        //    _channelMapper.CreateMap(channelMap);
                        //}
                    }
                }
                Thread.Sleep(500);
            }
        }

    }
}
