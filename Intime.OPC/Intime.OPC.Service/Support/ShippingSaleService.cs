using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class ShippingSaleService :BaseService, IShippingSaleService
    {
        private readonly IShippingSaleRepository _shippingSaleRepository;
        public ShippingSaleService(IShippingSaleRepository repository)
        {
            _shippingSaleRepository = repository;
        }

        public IList<OPC_ShippingSale> GetByShippingCode(string shippingCode)
        {
            return _shippingSaleRepository.GetByShippingCode(shippingCode);
        }

        public void Shipped(string saleOrderNo,int userID)
        {
           var lst=  _shippingSaleRepository.GetBySaleOrderNo(saleOrderNo);
            foreach (var e in lst)
            {
                e.ShippingStatus = EnumSaleOrderStatus.Shipped.AsID();
                e.UpdateDate = DateTime.Now;
                e.UpdateUser = userID;

                _shippingSaleRepository.Update(e);
            }
            
        }

        public void PrintExpress(string orderNo, int userId)
        {
            var lst = _shippingSaleRepository.GetBySaleOrderNo(orderNo);
            foreach (var e in lst)
            {
                e.ShippingStatus = EnumSaleOrderStatus.PrintExpress.AsID();
                e.UpdateDate = DateTime.Now;
                e.UpdateUser = userId;

                _shippingSaleRepository.Update(e);
            }
        }
    }
}
