﻿using System.Collections;
using System.Collections.Generic;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.RMA
{
    public interface IPackageService
    {
        //退货包裹管理的 退货包裹确认 查询收货单
        IList<SaleRmaDto> GetSaleRma(PackageReceiveDto packageReceiveDto);
        //退货包裹管理的 退货包裹确认 查询退货单
        IList<OPC_RMA> GetRma(PackageReceiveDto packageReceiveDto);
    }
}