using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Info
{
     [Export(typeof(ICommonInfo))]
    public class CommonInfo : ICommonInfo
    {
         /*物流公司*/
        public List<Domain.Models.ShipVia> GetShipViaList()
        {
            try
            {
                return new List<ShipVia>(RestClient.Get<ShipVia>("shipvia/getall"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
