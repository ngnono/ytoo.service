using System.Collections.Generic;
using OPCAPP.Domain.Dto;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ICommonInfo
    {
        List<ShipVia> GetShipViaList();
        IList<KeyValue> GetStoreList();
        IList<KeyValue> GetBrandList();

        IList<KeyValue> GetOrderStatus();
        IList<KeyValue<string>> GetPayMethod();
        IList<KeyValue> GetOutGoodsMehtod();
        IList<KeyValue> GetSectionList();
    }
}