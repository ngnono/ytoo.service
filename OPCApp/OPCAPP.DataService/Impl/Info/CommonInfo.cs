using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl.Info
{
    [Export(typeof (ICommonInfo))]
    public class CommonInfo : ICommonInfo
    {
        /*物流公司*/

        #region ICommonInfo Members

        public List<ShipVia> GetShipViaList()
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

        public IList<Store> GetStoreList()
        {
            try
            {
                return RestClient.Get<Store>("store/getall");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<Brand> GetBrandList()
        {
            try
            {
                return RestClient.Get<Brand>("brand/getall");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}