namespace Intime.OPC.Domain.Exception
{
    public class SaleOrderNotExistsException : System.Exception
    {
        public SaleOrderNotExistsException(string saleOrderNo) : base(string.Format("销售单不存在,销售单编号：{0}",saleOrderNo ))
        {
            this.SaleOrderNo = saleOrderNo;
           
        }

        public string SaleOrderNo { get; private set; }
    }


    public class SaleOrderNotFoundException : SaleOrderException
    {
        public SaleOrderNotFoundException(string msg)
            : base(msg)
        {
        }

        public string SaleOrderNo { get; private set; }
    }


    public class SaleOrderException : System.Exception
    {
        public SaleOrderException(string msg)
            : base(msg)
        {
        }

        public string SaleOrderNo { get; private set; }
    }
}