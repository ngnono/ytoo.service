using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCAPP.Domain.Dto
{
    public class KeyValue<TKey>:IEqualityComparer<KeyValue<TKey>>
    {
        public TKey Key { get; set; }

        public string Value { get; set; }

        public bool Equals(KeyValue<TKey> x, KeyValue<TKey> y)
        {
            if (x.Key.Equals(y.Key) && x.Value==y.Value)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(KeyValue<TKey> obj)
        {
            return string.Format("{0}_{1}", obj.Key.ToString(), obj.Value).GetHashCode();
        }
    }


    public class KeyValue : KeyValue<int>
    {
    }
}
