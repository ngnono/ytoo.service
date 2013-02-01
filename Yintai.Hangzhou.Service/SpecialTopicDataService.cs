using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request.SpecialTopic;
using Yintai.Hangzhou.Contract.DTO.Response.SpecialTopic;
using Yintai.Hangzhou.Contract.SpecialTopic;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Repository.Impl;

namespace Yintai.Hangzhou.Service
{
    public class SpecialTopicDataService : BaseService, ISpecialTopicDataService
    {
        private readonly ISpecialTopicRepository _specialTopicRepository;

        public SpecialTopicDataService(ISpecialTopicRepository specialTopicRepository)
        {
            _specialTopicRepository = specialTopicRepository;
        }


        public ExecuteResult<SpecialTopicCollectionResponse> GetList(GetSpecialTopicListRequest request)
        {
            int totalCount;

            var data = _specialTopicRepository.GetPagedList(request.PagerRequest, out totalCount, request.SortOrder,
                                                            request.Timestamp);


            var response = new SpecialTopicCollectionResponse(request.PagerRequest, totalCount)
                {
                    SpecialTopics = MappingManager.SpecialTopicInfoResponseMapping(data).ToList()
                };

            var result = new ExecuteResult<SpecialTopicCollectionResponse>(response);

            return result;
        }

        public ExecuteResult<SpecialTopicCollectionResponse> GetForRefresh(GetSpecialTopicListForRefresh request)
        {
            int totalCount;

            var data = _specialTopicRepository.GetPagedList(request.PagerRequest, out totalCount, request.SortOrder,
                                                            request.Timestamp);

            var response = new SpecialTopicCollectionResponse(request.PagerRequest, totalCount)
            {
                SpecialTopics = MappingManager.SpecialTopicInfoResponseMapping(data).ToList()
            };

            var result = new ExecuteResult<SpecialTopicCollectionResponse>(response);

            return result;
        }

        public ExecuteResult<SpecialTopicInfoResponse> GetInfo(GetSpecialTopicInfoRequest request)
        {
            var data = _specialTopicRepository.GetItem(request.TopicId);

            var response = MappingManager.SpecialTopicInfoResponseMapping(data);

            var result = new ExecuteResult<SpecialTopicInfoResponse>(response);

            return result;
        }
    }
}
