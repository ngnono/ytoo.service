﻿using System.Collections.Generic;

namespace OPCApp.Domain.Dto
{
    public class RoleMenuDto
    {
        public int RoleID { get; set; }

        public int UserID { get; set; }

        public IList<int> MeueList { get; set; }
    }
}