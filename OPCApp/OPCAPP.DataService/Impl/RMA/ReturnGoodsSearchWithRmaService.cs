using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.RMA;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto.ReturnGoods;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.RMA
{
    [Export(typeof(IReturnGoodsSearchWithRma))]
    public class ReturnGoodsSearchWithRmaService : IReturnGoodsSearchWithRma
    {
        #region 退货入收银
        public IList<RMADto> GetRmaForReturnCash(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto)
        {
            try
            {
                IList<RMADto> lst = RestClient.Get<RMADto>("trans/GetRmaCashByExpress", returnGoodsCommonSearchDto.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }

        public bool SetReturnGoodsCash(List<string> listRmaNo)
        {
            try
            {
                return RestClient.Post("trans/SetRmaCash", listRmaNo);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetReturnGoodsComplete(List<string> listRmaNo)
        {
            try
            {
                return RestClient.Post("trans/SetRmaCashOver", listRmaNo);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region 退货入库
        public bool SetReturnGoodsInStorage(List<string> listRmaNo)
        {
            throw new NotImplementedException();
        }
        public IList<RMADto> GetRmaForReturnInStorage(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region 打印退货单


        public bool PrintReturnGoods(List<string> listRmaNo)
        {
            throw new NotImplementedException();
        }

        public bool PrintReturnGoodsComplete(List<string> listRmaNo)
        {
            throw new NotImplementedException();
        }

        public IList<RMADto> GetRmaForReturnPrintDoc(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto)
        {
            throw new NotImplementedException();
        }

       

        #endregion

        #region 导购退货收货查询
        public IList<RMADto> GetRmaForShopperReturnOrReceivingPrintDoc(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 已经完成退货单查询
        public IList<RMADto> GetRmaForCompletedReturnGoods(ReturnGoodsCommonSearchDto returnGoodsCommonSearchDto)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}