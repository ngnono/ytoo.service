﻿using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IOrgService : IService,ICanAdd<OPC_OrgInfo>,ICanDelete,ICanUpdate<OPC_OrgInfo>
    {
        PageResult<OPC_OrgInfo> GetAll(int pageIndex, int pageSize = 20);

        OPC_OrgInfo AddOrgInfo(OPC_OrgInfo orgInfo);
    }
}