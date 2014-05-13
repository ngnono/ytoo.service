using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace Intime.OPC.Modules.Logistics.Print
{
    public interface IPrint
    {
        void Print(string xsdName, string rdlcName, PrintModel dtList, bool isPrint = false);
        void PrintExpress(string rdlcName, PrintExpressModel dtList, bool isPrint = false);
        void PrintFHD(string rdlcName, Order order, OPC_Sale opcSale, List<OPC_SaleDetail> listOpcSaleDetails, bool isPrint = false);
    }
}