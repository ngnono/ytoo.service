using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Exception
{
    /// <summary>
    ///     Class OrderNotExistsException.
    /// </summary>
    public class RoleNotExistsExcepion : System.Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleNotExistsExcepion" /> class.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        public RoleNotExistsExcepion(int roleId)
            : base(roleId.ToString())
        {
            this.RoleId = roleId.ToString();
        }

        public string RoleId { get; private set; }
    }
}
