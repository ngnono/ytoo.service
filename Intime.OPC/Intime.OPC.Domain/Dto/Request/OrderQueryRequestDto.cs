
using System;

namespace Intime.OPC.Domain.Dto.Request
{
    public class OrderQueryRequestDto:PageRequest
    {
        public string OrderNo { get; set; }

        public string OrderSource { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int BrandId { get; set; }

        public int StoreId { get; set; }

        public string PamentType { get; set; }

        public string OutGoodsType { get; set; }

        public string ExpressDeliveryCode { get; set; }

        public int ExpressCompanyId { get; set; }
    }

    public class StoreRequest 
    {
        public string NamePrefix { get; set; }

        public int? Status { get; set; }

        public int? SortOrder { get; set; }

        public string Name { get; set; }

        public int? Page { get; set; }

        public int? PageSize { get; set; }
    }

    public class PageRequest
    {
        private int pageIndex = 1;

        private int pageSize = 20;
        public int PageIndex
        {
            get
            {
                if (pageIndex <= 0)
                {
                    return 1;
                }
                return pageIndex;
            }
            set
            {
                pageIndex = value;
            }
        }

        public int PageSize
        {
            get
            {
                if (pageSize <= 0 || pageSize > 40)
                {
                    return 20;
                }
                return pageSize;
            }
            set
            {
                if (value >= 40)
                {
                    value = 40;
                }
                pageSize = value;
            }
        }
    }
}
