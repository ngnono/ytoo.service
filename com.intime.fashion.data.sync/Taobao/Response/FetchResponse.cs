using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.data.sync.Taobao.Response
{
    /// <summary>
    /// 分页显示Product
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FetchResponse<T>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 商品总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 列表数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
