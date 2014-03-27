using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SKU : IEntity
    {
        public int ProductId { get; set; }
        public int ColorValueId { get; set; }
        public int SizeValueId { get; set; }

        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}