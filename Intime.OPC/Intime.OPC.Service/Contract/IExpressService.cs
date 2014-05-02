using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service.Contract
{
    public interface IExpressService
    {
        void CreatePackage(ShippingSaleCreateDto package, int uid);
    }
}
