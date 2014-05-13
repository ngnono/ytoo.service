using System;
using System.Linq;
using Common.Logging;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Product.ProductSync.Models;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Repository;
/*
 
 *修改历史： 修改不同门店专柜码相同的问题，做映射关系时以 专柜码 - 门店.id作为映射key，
 *如果不存在则创建新的专柜。
 *上线前需注意：之前映射好的专柜需要把门店.id加在映射关系之后
 * Wallace 2014-05-12 
 * */
namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Processors
{
    /// <summary>
    /// 门店同步处理
    /// </summary>
    public class SectionSyncProcessor : ISectionSyncProcessor
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRemoteRepository _remoteRepository;
        private readonly IStoreSyncProcessor _storeSyncProcessor;
        private readonly IChannelMapper _channelMapper;

        public SectionSyncProcessor(IRemoteRepository remoteRepository, IStoreSyncProcessor storeSyncProcessor, IChannelMapper channelMapper)
        {
            _remoteRepository = remoteRepository;
            _storeSyncProcessor = storeSyncProcessor;
            _channelMapper = channelMapper;
        }

        public Section Sync(string channelSectionId, string channelStoreNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                // 检查专柜所属门店信息
                var storeExt = _storeSyncProcessor.Sync(channelStoreNo);
                if (storeExt == null)
                {
                    Log.ErrorFormat("同步专柜时发生错误,门店storeNo:{0}同步失败", channelStoreNo);
                    return null;
                }

                var mapKey = string.Format("{0}-{1}", channelSectionId, storeExt.Id);
                // 检查专柜映射关系是否存在
                var sectionMap = _channelMapper.GetMapByChannelValue(mapKey, ChannelMapType.SectionId);

                // 获取渠道门店信息
                var channelSection = _remoteRepository.GetSectionById(channelSectionId, channelStoreNo);

                if (channelSection == null)
                {
                    Log.ErrorFormat("远程获取专柜信息出错，counterId:{0},storeNo:{1}", channelSectionId, channelStoreNo);
                    return null;
                }

                if (sectionMap == null)
                {
                    var newSection = new Section()
                    {
                        CreateDate = DateTime.Now,
                        CreateUser = SystemDefine.SystemUser,
                        Location = string.Empty,
                        Name = channelSection.Name,
                        ContactPhone = string.Empty,
                        StoreId = storeExt.Id,
                        Status = 1,
                        UpdateDate = DateTime.Now,
                        UpdateUser = SystemDefine.SystemUser,
                    };

                    db.Sections.Add(newSection);
                    db.SaveChanges();

                    // 保存映射关系
                    var channelMap = new ChannelMap()
                    {
                        LocalId = newSection.Id,
                        ChannnelValue = mapKey,
                        MapType = ChannelMapType.SectionId
                    };

                    _channelMapper.CreateMap(channelMap);

                    return newSection;
                }

                var sectionExt = db.Sections.FirstOrDefault(s => s.Id == sectionMap.LocalId);

                if (sectionExt != null)
                {
                    sectionExt.UpdateDate = DateTime.Now;
                    sectionExt.UpdateUser = SystemDefine.SystemUser;
                    sectionExt.Name = channelSection.Name;
                    sectionExt.StoreId = storeExt.Id;

                    db.SaveChanges();
                }

                return sectionExt;
            }
        }
    }
}
