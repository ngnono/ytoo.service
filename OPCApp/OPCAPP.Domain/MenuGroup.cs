using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Domain
{
    public class MenuGroup
    {
        public int Sort { get; set; }

        public string Text { get; set; }

        public IList<MenuInfo> Items { get; set; }

    }

    public class MenuInfo {
        public int Sort { get; set; }
        public string Text { get; set; }
        public string ResourceUrl { get; set; }

    }
}
