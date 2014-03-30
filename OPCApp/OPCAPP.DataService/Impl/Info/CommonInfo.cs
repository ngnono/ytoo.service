using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Dto;
using OPCAPP.Domain.Enums;
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
                var lst= RestClient.Get<Store>("store/getall").Select<Store, KeyValue>((t) =>
                {
                    KeyValue kv=new KeyValue();
                    kv.Key = t.Id;
                    kv.Value = t.Name;
                    return kv;
                }).Distinct().ToList();
                lst.Insert(0, new KeyValue(-1,"全部"));
                return lst;
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
                var lst= RestClient.Get<Brand>("brand/getall").Select<Brand, KeyValue>((t) =>
                {
                    KeyValue kv = new KeyValue();
                    kv.Key = t.Id;
                    kv.Value = t.Name;
                    return kv;
                }).Distinct().ToList();
                lst.Insert(0, new KeyValue(-1, "全部"));
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<KeyValue> GetOrderStatus()
        {
            IList<KeyValue> lstKeyValues=new List<KeyValue>();
            lstKeyValues.Add(new KeyValue(-1, "全部"));
            lstKeyValues.Add(new KeyValue(0,"未发货"));
            lstKeyValues.Add(new KeyValue(5, "已发货"));
            return lstKeyValues;
        }

        #endregion


        public IList<KeyValue> GetOutGoodsMehtod()
        {
            IList<KeyValue> lstKeyValues = new List<KeyValue>();
            lstKeyValues.Add(new KeyValue(-1, "全部"));
            lstKeyValues.Add(new KeyValue(0, "快递"));
            return lstKeyValues;
        }
        public IList<KeyValue> GetPayMethod()
        {
            IList<KeyValue> lstKeyValues = new List<KeyValue>();
            lstKeyValues.Add(new KeyValue(-1, "全部"));
            lstKeyValues.Add(new KeyValue(0, "微信支付"));
            lstKeyValues.Add(new KeyValue(5, "支付宝"));
            return lstKeyValues;
        }
    }
}