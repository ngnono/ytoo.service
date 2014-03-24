<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;
=======
﻿using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> 57c447bbe22f6b2f14a2f332de06985506fd2e28

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
            return _saleRepository.UpdateSatus(sale);
        }
        public System.Collections.Generic.IList<OPC_Sale> Select()
        {
            return _saleRepository.Select();
        }

<<<<<<< HEAD
        public bool WriteSaleRemark(OPC_SaleComment comment)
        {
            return  _saleRemarkRepository.Create(comment);
        }

        public IList<SaleDto> GetNoPickUp(string saleId, int userId, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            //todo 权限校验

            
            var lst= _saleRepository.GetNoPickUp(saleId, orderNo, dtStart, dtEnd);
            var lstDto = new List<SaleDto>();
            foreach (var opcSale in lst)
            {
                var t = Mapper.Map<OPC_Sale, SaleDto>(opcSale);
                EnumSaleOrderStatus saleOrderStatus = (EnumSaleOrderStatus)opcSale.Status;
                t.StatusName = saleOrderStatus.GetDescription();
                lstDto.Add(t);
            }
            return lstDto;
        }
=======

>>>>>>> 57c447bbe22f6b2f14a2f332de06985506fd2e28

        public IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo)
        {
           return  _saleRemarkRepository.GetBySaleOrderNo(saleNo);
        }
    }
}
