using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Domain.Models
{
    public class Model : ValidatableBindableBase
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
    }
}
