using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.RMA;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;
using OPCApp.Domain.ReturnGoods;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (IPackageService))]
    public class PackageService : IPackageService
    {
        //退货包裹管理的 退货包裹确认 查询收货单
        public IList<SaleRmaDto> GetSaleRma(PackageReceiveDto packageReceiveDto)
        {
            try
            {
                IList<SaleRmaDto> lst = RestClient.Get<SaleRmaDto>("trans/GetSaleRmaByPack",
                    packageReceiveDto.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<SaleRmaDto>();
            }
        }

        //退货包裹管理的 退货包裹确认 查询退货单
        public IList<RMADto> GetRma(PackageReceiveDto packageReceiveDto)
        {
            try
            {
                IList<RMADto> lst = RestClient.Get<RMADto>("trans/GetRmaByPack", packageReceiveDto.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }


        public IList<RmaDetail> GetRmaDetailByRma(string rmaNo)
        {
            try
            {
                IList<RmaDetail> lst = RestClient.Get<RmaDetail>("rma/GetRmaDetailByRmaNo", string.Format("rmaNo={0}", rmaNo));
                return lst;
            }
            catch (Exception ex)
            {
                return new List<RmaDetail>();
            }
        }
        //物流确认收货
        public bool ReceivingGoodsSubmit(List<string> listRmoNo)
        {
            try
            {
                return RestClient.Post("rma/ShippingReceiveGoods", listRmoNo);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

      #region 退货包裹审核
        public bool TransVerifyPass(List<string> rmaNoList)
        {
            try
            {
                return RestClient.Post("custom/PackageVerify", new { RmaNos = rmaNoList, Pass = true });
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool TransVerifyNoPass(List<string> rmaNoList)
        {
            try
            {
                return RestClient.Post("custom/PackageVerify", new { RmaNos = rmaNoList, Pass = false });
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<RMADto> GetRmaByFilter(PackageReceiveDto packageReceive)
        {
            try
            {
                var lst = RestClient.Get<RMADto>("custom/GetRmaPackVerifyByPack", packageReceive.ToString());
                return lst;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }
      #endregion
      #region 退回打印快递单
        public IList<OPC_ShippingSale> GetShipListWithReturnGoods(RmaExpressDto rmaExpress)
        {
            throw new NotImplementedException();
        }

        public bool UpdateShipWithReturnExpress(RmaExpressSaveDto rmaExpressSaveDto)
        {
            throw new NotImplementedException();
        }

        public IList<RMADto> GetRmaForPrintExpress(string rmaNo)
        {
            throw new NotImplementedException();
        }

        public bool ShipPrintComplete(string rmaNO)
        {
            throw new NotImplementedException();
        }

        public bool ShipPrint(string rmaNo)
        {
            throw new NotImplementedException();
        }
      #endregion
        #region 完成快递单交接
        public IList<RMADto> GetRmaForPrintExpressConnect(string rmaNo)
        {
            throw new NotImplementedException();
        }

        public IList<Order> GetOrderForPrintExpressConnect(string orderNo)
        {
            throw new NotImplementedException();
        }

        #endregion
    
    }
}