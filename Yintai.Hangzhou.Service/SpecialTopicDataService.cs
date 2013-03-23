using System;
using System.Globalization;
using System.Linq;
using Yintai.Architecture.Common.Caching;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request.SpecialTopic;
using Yintai.Hangzhou.Contract.DTO.Response.SpecialTopic;
using Yintai.Hangzhou.Contract.SpecialTopic;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public class SpecialTopicDataService : BaseService, ISpecialTopicDataService
    {
        private readonly ISpecialTopicRepository _specialTopicRepository;

        public SpecialTopicDataService(ISpecialTopicRepository specialTopicRepository)
        {
            _specialTopicRepository = specialTopicRepository;
        }

        private ExecuteResult<SpecialTopicCollectionResponse> Get(PagerRequest pagerRequest, SpecialTopicSortOrder sortOrder, Timestamp timestamp)
        {
            var innerKey = String.Format("{0}_{1}_{2}", pagerRequest.ToString(), sortOrder,
                                 timestamp.ToString());
            string cacheKey;
            var s = CacheKeyManager.TopicListKey(out cacheKey, innerKey);

            var r = CachingHelper.Get(
              delegate(out SpecialTopicCollectionResponse data)
              {
                  var objData = CachingHelper.Get(cacheKey);
                  data = (objData == null) ? null : (SpecialTopicCollectionResponse)objData;

                  return objData != null;
              },
              () =>
              {
                  int totalCount;

                  var data = _specialTopicRepository.GetPagedList(pagerRequest, out totalCount, sortOrder,
                                                                  timestamp);

                  var response = new SpecialTopicCollectionResponse(pagerRequest, totalCount)
                  {
                      SpecialTopics = MappingManager.SpecialTopicInfoResponseMapping(data).ToList()
                  };

                  return response;
              },
              data =>
              CachingHelper.Insert(cacheKey, data, s));

            var result = new ExecuteResult<SpecialTopicCollectionResponse> { Data = r };

            return result;
        }

        public ExecuteResult<SpecialTopicCollectionResponse> GetList(GetSpecialTopicListRequest request)
        {
            return Get(request.PagerRequest, request.SortOrder, request.Timestamp);
        }

        public ExecuteResult<SpecialTopicCollectionResponse> GetForRefresh(GetSpecialTopicListForRefresh request)
        {
            return Get(request.PagerRequest, request.SortOrder, request.Timestamp);
        }

        public ExecuteResult<SpecialTopicInfoResponse> GetInfo(GetSpecialTopicInfoRequest request)
        {
            var innerKey = request.TopicId.ToString(CultureInfo.InvariantCulture);
            string cacheKey;
            var s = CacheKeyManager.TopicInfoKey(out cacheKey, innerKey);

            var r = CachingHelper.Get(
              delegate(out SpecialTopicInfoResponse data)
              {
                  var objData = CachingHelper.Get(cacheKey);
                  data = (objData == null) ? null : (SpecialTopicInfoResponse)objData;

                  return objData != null;
              },
              () =>
              {
                  var data = _specialTopicRepository.GetItem(request.TopicId);
                  var response = MappingManager.SpecialTopicInfoResponseMapping(data);

                  return response;
              },
              data =>
              CachingHelper.Insert(cacheKey, data, s));

            var result = new ExecuteResult<SpecialTopicInfoResponse>(r);

            return result;
        }
    }
}
