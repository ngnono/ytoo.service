﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Dto
{
    public  class RoleAuthDto
    {
        public int  RoleID { get; set; }

        public int UserID { get; set; }

        /*菜单IDS 或者 用户IDS*/
        public IList<int> IDList { get; set; }
    }
}
