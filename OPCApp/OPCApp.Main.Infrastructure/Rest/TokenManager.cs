using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Rest
{
    [Export(typeof(TokenManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TokenManager
    {
        public string Token { get; set; }
    }
}
