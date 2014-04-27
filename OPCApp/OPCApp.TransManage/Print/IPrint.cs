using System.Collections.Generic;
using OPCApp.Domain.Models;

namespace OPCApp.TransManage.Print
{
    public interface IPrint
    {
        void Print(string xsdName, string rdlcName, PrintModel dtList, bool isFast);
        void PrintExpress(string rdlcName, PrintExpressModel dtList);
        void PrintFHD(string rdlcName,Order order, OPC_Sale opcSale,List<OPC_SaleDetail> listOpcSaleDetails );
    }
}