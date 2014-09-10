using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.intime.fashion.data.sync.Taobao.Request;
using com.intime.fashion.data.sync.Taobao.Response;
using Yintai.Hangzhou.Model.ES;

namespace com.intime.fashion.data.sync.Taobao.Fetcher
{
    public interface IProductFetcher
    {
        FetchResponse<ESProduct> Fetch(FetchRequest request);
    }
}
