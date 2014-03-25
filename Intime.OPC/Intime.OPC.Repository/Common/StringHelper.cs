using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Repository.Common
{
    public class StringHelper
    {
        public static int[] ToInts(string s, char pchar = '%')
        {
            IList<int> lst = new List<int>();
            string[] strs = s.Split(pchar);
            foreach (string t in strs)
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