using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.TransManage.Print
{
    public interface IPrint
    {
        void Print(string xsdName, string rdlcName, PrintModel dtList, bool isFast);
    }
}
