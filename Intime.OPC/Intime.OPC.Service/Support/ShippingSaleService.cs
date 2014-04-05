using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class ShippingSaleService :BaseService<OPC_ShippingSale>, IShippingSaleService
    {
        private readonly IShippingSaleRepository _shippingSaleRepository;
        public ShippingSaleService(IShippingSaleRepository repository):base(repository)
        {
            _shippingSaleRepository = repository;
        }

        public PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode,int pageIndex,int pageSize=20)
        {
            return _shippingSaleRepository.GetByShippingCode(shippingCode,pageIndex,pageSize);
        }

        public void Shipped(string saleOrderNo,int userID)
        {
           var lst=  _shippingSaleRepository.GetBySaleOrderNo(saleOrderNo);

           if (lst == null)
           {
               throw new ShippingSaleNotExistsException(saleOrderNo);
           }
           lst.ShippingStatus = EnumSaleOrderStatus.Shipped.AsID();
           lst.UpdateDate = DateTime.Now;
           lst.UpdateUser = userID;

           _shippingSaleRepository.Update(lst);
           
            
        }

        public void PrintExpress(string orderNo, int userId)
        {
            //todo  确定是销售单还是订单
            var lst = _shippingSaleRepository.GetBySaleOrderNo(orderNo);
            if (lst==null)
            {
                throw new ShippingSaleNotExistsException(orderNo);
            }
                lst.ShippingStatus = EnumSaleOrderStatus.PrintExpress.AsID();
                lst.UpdateDate = DateTime.Now;
                lst.UpdateUser = userId;

                _shippingSaleRepository.Update(lst);
            
        }
    }
}
