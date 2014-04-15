using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Dto
{
    //�˻��������� ��Ӧ��ݵ�
    public class RmaExpressRequest : BaseRequest
    {

        public string OrderNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void FormateDate()
        {
            StartDate = StartDate.Date;
            EndDate = EndDate.Date.AddDays(1);
        }

    }

    public class RmaExpressSaveDto
    {
        /*��ݹ�˾ID*/
        public int ShipViaID { get; set; }
        /*��ݹ�˾*/
        public string ShipViaName { get; set; }
        /*ʵ���˷�*/
        public double ShippingFee { get; set; }
        /*��ݵ���*/
        public string ShippingCode { get; set; }
        /*�˻���*/
        public string RmaNo { get; set; }
    }

}