
using System;

namespace com.intime.fashion.data.sync.Taobao.Request
{
    public class FetchRequest
    {
        public string Channel { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}
