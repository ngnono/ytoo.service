using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Models;

namespace OPCApp.Domain
{
    public class MenuGroup
    {
        public MenuGroup() {
            this.Items = new List<OPC_AuthMenu>();
        }

        public int Id { get; set; }
        public int Sort { get; set; }

        public string MenuName { get; set; }

        public IList<OPC_AuthMenu> Items { get; set; }

    }

   
}
