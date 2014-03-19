using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Dto
{
    public  class RoleMenuDto
    {
        public int  RoleID { get; set; }

        public int UserID { get; set; }

        public IList<int> MeueList { get; set; }
    }
}
