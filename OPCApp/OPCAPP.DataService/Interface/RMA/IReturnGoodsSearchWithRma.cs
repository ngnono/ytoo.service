using System.Collections.Generic;
using Intime.OPC.ApiClient.Annotations;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.ReturnGoods;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;


namespace OPCApp.DataService.Interface.RMA
{
    public interface IReturnGoodsSearchWithRma
    {
        //退货入收银
        IList<RMADto> GetRmaForReturnCash(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
        bool SetReturnGoodsCash(List<string>listRmaNo);
        bool SetReturnGoodsComplete(List<string> listRmaNo);

    }
}