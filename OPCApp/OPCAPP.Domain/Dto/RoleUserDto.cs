using System.Collections.Generic;

namespace OPCApp.Domain.Dto
{
    public class RoleUserDto
    {
        public RoleUserDto()
        {
            UserIds = new List<int>();
        }

        public int RoleId { get; set; }

        public IList<int> UserIds { get; set; }
    }
}