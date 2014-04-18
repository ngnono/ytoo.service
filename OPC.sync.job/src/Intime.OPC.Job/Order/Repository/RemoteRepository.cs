using System;
using System.Collections.Generic;
using System.Linq;

using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Domain;
using Intime.O2O.ApiClient.Request;
using Intime.O2O.ApiClient.Response;
using Intime.OPC.Job.Order.Models;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Job.Order.Repository
{
    public class RemoteRepository : IRemoteRepository
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IApiClient _apiClient;

        static RemoteRepository()
        {
            AutoMapper.Mapper.CreateMap<OrderStatus, OrderStatusDto>();

            AutoMapper.Mapper.CreateMap<Head, HeadDto>();
            AutoMapper.Mapper.CreateMap<Detail, DetailDto>();
            AutoMapper.Mapper.CreateMap<PayMent, PayMentDto>();

            AutoMapper.Mapper.CreateMap<OrderStatusResult, OrderStatusResultDto>();

        }

        public RemoteRepository()
        {
            _apiClient = new DefaultApiClient();
        }

        public RemoteRepository(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public OrderStatusResultDto GetOrderStatusById(string Id, string storeNo)
        {
            var result = _apiClient.Post(new GetOrderStatusByIdRequest()
            {
                Data = new GetOrderStatusByIdRequestData()
                {
                    Id=Id,
                    StoreNo = storeNo
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取门店信息出错,message:{0}", result.Message);
                return null;
            }

            if (result.Data == null || string.IsNullOrEmpty(result.Data.Id))
            {
                Log.ErrorFormat("信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<OrderStatusResult, OrderStatusResultDto>(result.Data);
        }
    }
}
