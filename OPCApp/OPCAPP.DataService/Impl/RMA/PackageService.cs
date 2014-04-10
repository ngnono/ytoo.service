﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (IPackageService))]
    public class PackageService : IPackageService
    {
        //退货包裹管理的 退货包裹确认 查询收货单
        public IList<SaleRmaDto> GetSaleRma(PackageReceiveDto packageReceiveDto)
        {
            try
            {
                IList<SaleRmaDto> lst = RestClient.Get<SaleRmaDto>("trans/GetSaleRmaByPack",
                    packageReceiveDto.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<SaleRmaDto>();
            }
        }

        //退货包裹管理的 退货包裹确认 查询退货单
        public IList<OPC_RMA> GetRma(PackageReceiveDto packageReceiveDto)
        {
            try
            {
                IList<OPC_RMA> lst = RestClient.Get<OPC_RMA>("trans/GetRmaByPack", packageReceiveDto.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<OPC_RMA>();
            }
        }
    }
}