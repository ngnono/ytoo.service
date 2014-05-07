using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// Counter
    /// </summary>
    public class Counter : Dimension
    {
        /// <summary>
        /// Brands
        /// </summary>
        public IList<Brand> Brands { get; set; }

        /// <summary>
        /// Organization
        /// </summary>
        public Organization Organization { get; set; }
    }
}
