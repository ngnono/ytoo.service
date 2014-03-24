﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ITransService
    {
        bool SetStatusAffirmPrintSaleFinish(IList<string> saleOrderNoList);
        bool SetStatusStoreInSure(IList<string> saleOrderNoList);
        bool SetStatusSoldOut(IList<string> saleOrderNoList);
        bool SetStatusPrintExpress(IList<string> saleOrderNoList);
        bool SetStatusPrintInvoice(IList<string> saleOrderNoList);
        PageResult<OPC_Sale> Search(string salesfilter, EnumSearchSaleStatus searchSaleStatus);
        PageResult<OPC_SaleDetail> SelectSaleDetail(string saleOrderNo);
    }
}