using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto
{
    public class KeyValue<TKey>
    {
        public TKey Key { get; set; }

        public string Value { get; set; }
    }


    public class KeyValue : KeyValue<int>
    {
    }
}
