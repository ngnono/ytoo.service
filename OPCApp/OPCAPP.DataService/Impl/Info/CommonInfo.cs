using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Dto;
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

        public IList<KeyValue> GetStoreList()
        {
            try
            {
                return RestClient.Get<Store>("store/getall").Select<Store, KeyValue>((t) =>
                {
                    KeyValue kv=new KeyValue();
                    kv.Key = t.Id;
                    kv.Value = t.Name;
                    return kv;
                }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<KeyValue> GetBrandList()
        {
            try
            {
                return RestClient.Get<Brand>("brand/getall").Select<Brand, KeyValue>((t) =>
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = t.Id;
                    kv.Value = t.Name;
                    return kv;
                }).Distinct().ToList(); ;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}