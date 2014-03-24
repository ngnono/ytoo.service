using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public  class MapConfig
    {
        public static void Config()
        {
            var map=  Mapper.CreateMap<OPC_Sale, SaleDto>();
           
        }
    }
}
