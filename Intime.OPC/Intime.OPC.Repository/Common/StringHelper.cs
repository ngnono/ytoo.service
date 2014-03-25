using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository.Common
{
    public class StringHelper
    {
        public static int[] ToInts( string s, char pchar = '%')
        {
            IList<int> lst = new List<int>();
            var strs = s.Split(pchar);
            foreach (var t in strs)
            {
                int i = 0;
                bool bl = int.TryParse(t, out i);
                if (bl)
                {
                    lst.Add(i);
                }
            }
         
            return lst.ToArray();
        }
    }
}
