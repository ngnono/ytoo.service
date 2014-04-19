using Common.Logging;
using Intime.O2O.ApiClient;
using Intime.O2O.ApiClient.Domain;
using Intime.O2O.ApiClient.Request;
using Intime.OPC.Domain.Models;
using Intime.OPC.Job.Order.Models;
using System.Linq;

namespace Intime.OPC.Job.Order.Repository
{
    public class OrderRemoteRepository : IOrderRemoteRepository
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IApiClient _apiClient;

        static OrderRemoteRepository()
        {
            AutoMapper.Mapper.CreateMap<OrderStatus, OrderStatusDto>();
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

            if (result.Data == null || string.IsNullOrEmpty(result.Data.Id))
            {
                Log.ErrorFormat("信息为空,message:{0}", result.Message);
                return null;
            }

            return AutoMapper.Mapper.Map<OrderStatusResult, OrderStatusResultDto>(result.Data);
        }
    }
}
