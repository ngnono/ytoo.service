using System;


namespace Intime.OPC.Domain.Dto
{
    //退货包裹管理 打应快递单
    public class RmaExpressDto
    {

        public string OrderNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
    }

    public class RmaExpressSaveDto
    {
        /*快递公司ID*/
        public int ShipViaID { get; set; }
        /*快递公司*/
        public string ShipViaName { get; set; }
        /*实际运费*/
        public double ShippingFee { get; set; }
        /*快递单号*/
        public string ShippingCode { get; set; }
        /*退货单*/
        public string RmaNo { get; set; }
    }

}