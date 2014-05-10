using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Dto.Request
{
    public class ExpressRequestDto : DateRangeRequest
    {
        private int pageSize = 0;
        private int pageIndex = 0;
        public string OrderNo { get; set; }

        public int PageIndex
        {
            get {
                if (pageIndex <= 0)
                {
                    return 1;
                }
                return pageSize;
            }
        }

        public int PageSize { get{
            if (pageIndex <= 0 || pageSize > 20)
            {
                return 20;
            }
            return pageSize;
        } }
    }
}
