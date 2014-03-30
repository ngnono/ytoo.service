using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        IList<KeyValue> GetPayMethod();
        IList<KeyValue> GetOutGoodsMehtod();
  }
}
