using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Models
{
    /// <summary>
    /// Brand
    /// </summary>
    public class Brand : Dimension
    {
        private bool enabled;

        /// <summary>
        /// English name
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Enabled or disabled
        /// </summary>
        public bool Enabled 
        {
            get { return enabled; }
            set { SetProperty(ref enabled, value); }
        }

        /// <summary>
        /// Supplier
        /// </summary>
        public Supplier Supplier { get; set; }

        /// <summary>
        /// Counters on sale
        /// </summary>
        public IList<Counter> Counters { get; set; }
    }
}
