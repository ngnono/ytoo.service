using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace OPCApp.ReturnGoodsManage.Print
{
    public interface IPrint
    {
        void Print(string xsdName, string rdlcName, PrintModel dtList, bool isPrint = false);
        void PrintExpress(string rdlcName, PrintExpressModel dtList, bool isPrint = false);
        void PrintDeliveryOrder(string rdlcName, Order order, IList<OPC_RMA> opcRma, IList<OPC_RMADetail> listOpcRmaDetails, bool isPrint = false);
    }
}