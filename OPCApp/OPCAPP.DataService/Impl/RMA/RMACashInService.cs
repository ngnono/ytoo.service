using System.Collections.Generic;
using System.ComponentModel.Composition;
using  OPCApp.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (IRMACashInService))]
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
            IList<OPC_RMA> lst = RestClient.Get<OPC_RMA>(url, filter);
            return new PageResult<OPC_RMA>(lst, lst.Count);
        }

        public PageResult<OPC_RMADetail> GetRMADetailByRMANo(string rmaNo)
        {
            IList<OPC_RMADetail> lst = RestClient.Get<OPC_RMADetail>("RMA/GetRMADetailByRMANo", rmaNo);
            return new PageResult<OPC_RMADetail>(lst, lst.Count);
        }

        public bool FinishRMACashIn(OPC_RMA RMASelect)
        {
            return RestClient.Put("RMA/FinishRMACashIn", RMASelect);
        }
    }
}