using OPCApp.Domain;
using OPCApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.DataService.Interface
{
    public interface IRoleDataService : OPCApp.Infrastructure.DataService.IBaseDataService<OPC_AuthRole>
    {
    }
}
