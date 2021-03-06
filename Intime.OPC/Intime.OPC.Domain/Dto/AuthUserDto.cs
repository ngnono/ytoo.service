﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Dto
{
    public class AuthUserDto
    {
        public string Name { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public string LogonName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool? IsValid { get; set; }
        public string OrgId { get; set; }

        public string OrgName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }

        public string DataAuthId { get; set; }

        public string DataAuthName { get; set; }

        public int Id { get; set; }

    }
}
