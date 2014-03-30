using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto
{
    public class KeyValue<TKey>
    {
        public KeyValue()
        {
        }

        public KeyValue(TKey key,string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key { get; set; }

        public string Value { get; set; }

      
    }


    public class KeyValue :KeyValue<int>
    {
        public KeyValue() : base()
        {
        }

        public KeyValue(int key, string value) : base(key, value)
        {
        }
    }
}
