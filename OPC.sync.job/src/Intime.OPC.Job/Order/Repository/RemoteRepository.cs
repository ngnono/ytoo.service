using System;
using System.Collections.Generic;
using System.Linq;

using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Domain;
using Intime.O2O.ApiClient.Request;
using Intime.O2O.ApiClient.Response;
using Intime.OPC.Job.Order.Models;

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

        public OrderStatusDto GetOderStatus(OrderStatusDetail orderStatusDetail)
        {
            var result = _apiClient.Post(new GetOrderStatusRequest()
            {
                Data = new GetOrderStatusRequestData()
                {
                    Id = orderStatusDetail.Id,
                    Status = orderStatusDetail.Status,
                    Head = new Head {
                        Id = orderStatusDetail.Head.Id,
                        MainId = orderStatusDetail.Head.MainId,
                        Flag = orderStatusDetail.Head.Flag,
                        CreateTime = orderStatusDetail.Head.CreateTime,
                        PayTime = orderStatusDetail.Head.PayTime,
                        Type = orderStatusDetail.Head.Type,
                        Status = orderStatusDetail.Head.Status,
                        Quantity = orderStatusDetail.Head.Quantity,
                        Discount = orderStatusDetail.Head.Discount,
                        Total = orderStatusDetail.Head.Total,
                        VipNo = orderStatusDetail.Head.VipMemo,
                        VipMemo = orderStatusDetail.Head.VipMemo,
                        StoreNo = orderStatusDetail.Head.StoreNo,
                        OldId = orderStatusDetail.Head.OldId,
                        OperId = orderStatusDetail.Head.OperId,
                        OperName = orderStatusDetail.Head.OperName,
                        OperTime = orderStatusDetail.Head.OperTime
                    },
                    Detail = new Detail{
                        Id = orderStatusDetail.Detail.Id,
                        ProductId = orderStatusDetail.Detail.ProductId,
                        ProductName = orderStatusDetail.Detail.ProductName,
                        Price = orderStatusDetail.Detail.Price,
                        Discount = orderStatusDetail.Detail.Discount,
                        VipDiscount = orderStatusDetail.Detail.VipDiscount,
                        Quantity = orderStatusDetail.Detail.Quantity,
                        Total = orderStatusDetail.Detail.Total,
                        RowNo = orderStatusDetail.Detail.RowNo,
                        ComCode = orderStatusDetail.Detail.ComCode,
                        Counter = orderStatusDetail.Detail.Counter,
                        Memo = orderStatusDetail.Detail.Memo,
                        StoreNo = orderStatusDetail.Detail.StoreNo
                    },
                    PayMent = new PayMent{
                        Id = orderStatusDetail.PayMent.Id,
                        Type = orderStatusDetail.PayMent.Type,
                        TypeId = orderStatusDetail.PayMent.TypeId,
                        TypeName = orderStatusDetail.PayMent.TypeName,
                        No = orderStatusDetail.PayMent.No,
                        Amount = orderStatusDetail.PayMent.Amount,
                        RowNo = orderStatusDetail.PayMent.RowNo,
                        Memo = orderStatusDetail.PayMent.Memo,
                        StoreNo = orderStatusDetail.PayMent.StoreNo

                    }
                }

            });

            if (!result.Status)
            {
                Log.ErrorFormat("获取信息出错,message:{0}", result.Message);
                return null;
            }

            if (result.Data == null || string.IsNullOrEmpty(result.Data.Id))
            {
                Log.ErrorFormat("信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<OrderStatus, OrderStatusDto>(result.Data);
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
