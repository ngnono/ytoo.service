using System.Collections.Generic;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.ReturnGoods;

namespace OPCApp.DataService.Interface.RMA
{
    public interface IReturnGoodsSearchWithRma
    {
        //退货入收银
        IList<RMADto> GetRmaForReturnCash(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
        bool SetReturnGoodsCash(List<string> listRmaNo);
        bool SetReturnGoodsComplete(List<string> listRmaNo);
        //退货入库
        bool SetReturnGoodsInStorage(List<string> listRmaNo);
        IList<RMADto> GetRmaForReturnInStorage(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
        //打印退货单
        bool PrintReturnGoods(List<string> listRmaNo);
        bool PrintReturnGoodsComplete(List<string> listRmaNo);
        IList<RMADto> GetRmaForReturnPrintDoc(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
        //导购退货收货查询
        IList<RMADto> GetRmaForShopperReturnOrReceivingPrintDoc(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
        //已完成退货单查询
        IList<RMADto> GetRmaForCompletedReturnGoods(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto);
    }
}