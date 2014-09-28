using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace com.intime.o2o.data.exchange.Tmall.Product.Tools
{
    public class Util
    {
        public IDictionary<string, string> Zip(List<string> a, ArrayList b)
        {
            if (a == null || b == null)
            {
                return new Dictionary<string, string>();
            }

            var count = Math.Min(a.Count, b.Count);

            var dict = new Dictionary<string, string>();
            for (var i = 0; i < count; i++)
            {
                if (!dict.ContainsKey(a[i].ToString(CultureInfo.InvariantCulture)))
                    dict.Add(a[i].ToString(CultureInfo.InvariantCulture), b[i].ToString());
            }

            return dict;
        }

        public string GetValue(IDictionary<string, string> dic, string key)
        {
            return dic[key];
        }
    }
}
