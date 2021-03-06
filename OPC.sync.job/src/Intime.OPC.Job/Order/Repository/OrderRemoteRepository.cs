﻿using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Domain;
using Intime.O2O.ApiClient.Request;
using Intime.OPC.Domain.Models;
using System.Linq;
using Intime.OPC.Job.Order.DTO;

namespace Intime.OPC.Job.Order.Repository
{
    public class OrderRemoteRepository : IOrderRemoteRepository
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IApiClient _apiClient;

        static OrderRemoteRepository()
        {
            AutoMapper.Mapper.CreateMap<OrderStatusResult, OrderStatusResultDto>();
        }

        public OrderRemoteRepository()
        {
            _apiClient = new DefaultApiClient();
        }

        public OrderRemoteRepository(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public OrderStatusResultDto GetOrderStatusById(OPC_Sale saleOrder)
        {
            OPC_Stock opcStock; ;

            using (var db = new YintaiHZhouContext())
            {
                opcStock = db.OPC_Stock.FirstOrDefault(a => a.SectionId == saleOrder.SectionId);
            }

            if (opcStock == null)
            {
                throw new StockNotExistsException(string.Format(""));
            }

            var result = _apiClient.Post(new GetOrderStatusByIdRequest()
            {
                Data = new
                {
                    id = saleOrder.SaleOrderNo,
                    storeno = opcStock.StoreCode
                }
            });

            if (!result.Status)
            {
                Log.ErrorFormat("查询订单信息失败,message:{0}", result.Message);
                return null;
            }
            return AutoMapper.Mapper.Map<OrderStatusResult, OrderStatusResultDto>(result.Data);
        }
    }
}
