using System;
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
using OPCAPP.Domain.Enums;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof(IRMACashInService))]
    public class RMACashInService : IRMACashInService
    {

        public PageResult<OPC_RMA> GetRMA(string filter, EnumRMACashStatus enumRMACashStatus)
        {
            string url = "";
            switch (enumRMACashStatus)
            {
                case EnumRMACashStatus.NoCash:
                    url = "RMA/GetRMANoCash";
                    break;
            }
            var lst = RestClient.Get<OPC_RMA>(url, filter);
            return new PageResult<OPC_RMA>(lst, lst.Count);
        }

        public PageResult<OPC_RMADetail> GetRMADetailByRMANo(string rmaNo)
        {
            var lst = RestClient.Get<OPC_RMADetail>("RMA/GetRMADetailByRMANo", rmaNo);
            return new PageResult<OPC_RMADetail>(lst, lst.Count);
        }

        public bool FinishRMACashIn(OPC_RMA RMASelect)
        {
            return RestClient.Put("RMA/FinishRMACashIn", RMASelect);
        }

       
    }
}
