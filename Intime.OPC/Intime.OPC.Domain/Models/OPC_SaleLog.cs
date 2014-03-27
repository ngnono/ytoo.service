using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleLog : IEntity
    {
        public string SaleOrderNo { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }

        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}