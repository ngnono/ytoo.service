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
        private bool repealed;

        /// <summary>
        /// Counter code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Area code
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Repealed or not
        /// </summary>
        public bool Repealed
        {
            get { return repealed; }
            set { SetProperty(ref repealed, value); }
        }

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
