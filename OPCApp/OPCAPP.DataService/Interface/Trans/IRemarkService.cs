using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface.Trans
{
    public interface IRemarkService
    {
        bool WriteRemark(OPC_SaleComment saleComment);
        PageResult<OPC_SaleComment> SelectRemark(string selectRemarkIdsAndType);
    }
}
