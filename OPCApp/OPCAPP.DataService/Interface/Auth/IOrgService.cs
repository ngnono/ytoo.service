using System.Collections.Generic;
using Intime.OPC.ApiClient.Annotations;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface
{
    public interface IOrgService : IBaseDataService<OPC_OrgInfo>
    {
        IList<OPC_OrgInfo> Search();
    }
}