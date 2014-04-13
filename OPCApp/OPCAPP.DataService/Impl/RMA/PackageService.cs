﻿using System;
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
                var lst = RestClient.GetPage<SaleRmaDto>("trans/GetSaleRmaByPack",
                    packageReceiveDto.ToString());
                return lst.Result;
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
                var lst = RestClient.GetPage<RMADto>("trans/GetRmaByPack", packageReceiveDto.ToString());
                return lst.Result;
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
                var lst = RestClient.GetPage<RmaDetail>("rma/GetRmaDetailByRmaNo", string.Format("rmaNo={0}&pageIndex={1}&pageSize={2}", rmaNo,1,300));
                return lst.Result;
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
                return RestClient.Post("custom/ShippingReceiveGoods", listRmoNo);
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
                var lst = RestClient.GetPage<RMADto>("custom/GetRmaPackVerifyByPack", packageReceive.ToString());
                return lst.Result;
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
            try
            {
                var lst = RestClient.GetPage<OPC_ShippingSale>("custom/GetRmaByPackPrintPress", rmaExpress.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_ShippingSale>();
            }
        }

        public bool UpdateShipWithReturnExpress(RmaExpressSaveDto rmaExpressSaveDto)
        {
            try
            {
                return RestClient.Post("custom/UpdateShipRmaVia",rmaExpressSaveDto);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IList<RMADto> GetRmaForPrintExpress(string rmaNo)
        {
            try
            {
                var lst = RestClient.GetPage<RMADto>("custom/GetRmaByPackPrintPress", string.Format("rmaNo={0}&pageIndex={1},pageSize={2}",rmaNo,1,300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }

        public bool ShipPrintComplete(string shippingCode)
        {
            try
            {
                return RestClient.Post("custom/PintRmaShipping", new { shippingCode = shippingCode });
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ShipPrint(string shippingCode)
        {
            try
            {
                return RestClient.Post("custom/PintRmaShippingOver", new { shippingCode = shippingCode });
            }
            catch (Exception ex)
            {
                return false;
            }
        }
      #endregion
        #region 完成快递单交接
        public IList<RMADto> GetRmaForPrintExpressConnect(string rmaNo)
        {
            try
            {
                var lst = RestClient.GetPage<RMADto>("custom/GetRmaByPackPrintPress", string.Format("rmaNo={0}&pageIndex={1},pageSize={2}", rmaNo, 1, 300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<RMADto>();
            }
        }

        public IList<Order> GetOrderForPrintExpressConnect(string orderNo)
        {
            try
            {
                var order = RestClient.GetSingle<Order>("order/GetOrderByOderNo", string.Format("orderNo={0}", orderNo));
                return  new List<Order>(){ order};
            }
            catch (Exception ex)
            {
                return new List<Order>();
            }
        }

        public IList<OPC_ShippingSale> GetShipListWithReturnGoodsConnect(RmaExpressDto rmaExpress)
        {
            try
            {
                var lst = RestClient.GetPage<OPC_ShippingSale>("custom/GetRmaShippingPrintedByPack", rmaExpress.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_ShippingSale>();
            }
        }
        public bool ShipPrintComplateConnect(string shippingCode)
        {
            try
            {
                return RestClient.Post("custom/PintRmaShippingOverConnect", new { shippingCode = shippingCode });
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}