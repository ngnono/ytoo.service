namespace Intime.OPC.Domain.Exception
{
    public class SaleOrderNotExistsException : System.Exception
    {
        public SaleOrderNotExistsException(string saleOrderNo) : base(saleOrderNo)
        {
            this.SaleOrderNo = saleOrderNo;
        }

        public string SaleOrderNo { get; private set; }
    }
}