using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public class OPC_RMALog : IEntity
    {
        public int OpcRmaId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string Operation { get; set; }

        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}