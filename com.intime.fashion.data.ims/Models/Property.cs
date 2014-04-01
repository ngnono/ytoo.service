using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class PropertyEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public Nullable<int> OrderFlag { get; set; }
        public bool IsSaleProperty { get; set; }
        public bool IsKeyProperty { get; set; }
        public bool IsNecessary { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {       
                get { return Id; }
 
        }

        #endregion
    }
}
