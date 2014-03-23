using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Service.Support
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ISaleRemarkRepository _saleRemarkRepository;
        public SaleService(ISaleRepository saleRepository,ISaleRemarkRepository saleRemarkRepository)
        {
            _saleRepository = saleRepository;
            _saleRemarkRepository = saleRemarkRepository;
        }

        public bool UpdateSatus(OPC_Sale sale)
        {
            return false;
            // return _saleRepository.UpdateSatus(sale);
        }
        public System.Collections.Generic.IList<OPC_Sale> Select()
        {
            return _saleRepository.Select();
        }

        public IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo)
        {
           return  _saleRemarkRepository.GetBySaleOrderNo(saleNo);
        }

        public bool PrintSale(string saleNo, int userId)
        {
            var saleOrder = _saleRepository.GetBySaleNo(saleNo);
            if (saleOrder == null)
            {
                throw new SaleOrderNotExistsException(saleNo);
            }
            
            if (saleOrder.Status >= (int)EnumSaleOrderStatus.PrintSale)
            {
                return true;
            }
            saleOrder.PrintTimes += 1;
            saleOrder.UpdatedDate = DateTime.Now;
            saleOrder.UpdatedUser = userId;
            _saleRepository.Update(saleOrder);
            return true;
        }

        public IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo, int userId)
        {
            if (string.IsNullOrEmpty(saleOrderNo))
            {
                throw new ArgumentNullException("saleOrderNo");
            }
            var saleOrder = _saleRepository.GetBySaleNo(saleOrderNo);
            if (saleOrder == null)
            {
                throw new SaleOrderNotExistsException(saleOrderNo);
            }

            //todo 权限校验

            return _saleRepository.GetSaleOrderDetails(saleOrderNo);
        }
    }
}
