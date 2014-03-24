﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using Intime.OPC.ApiClient;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof(IRemarkService))]
    public class RemarkSalesService : IRemarkService
    {

        public bool WriteRemark(OPC_SaleComment saleComment)
        {
            bool bFalg = RestClient.Put("sale/writeremark", saleComment);
            return bFalg;
        }

        PageResult<OPC_SaleComment> IRemarkService.SelectRemark(string saleOrderNo)
        {
            var lst = RestClient.Get<OPC_SaleComment>("sale/selectremark",string.Format("Id={0}",saleOrderNo));
            return new PageResult<OPC_SaleComment>(lst, lst.Count);
        }
    }
}