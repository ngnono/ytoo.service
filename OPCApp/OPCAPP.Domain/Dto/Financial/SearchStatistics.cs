using System;

namespace OPCAPP.Domain.Dto.Financial
{
    public class SearchStatistics
    {
        public SearchStatistics()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override string ToString()
        {
            return string.Format("StartTime={0}&EndTime={1}&pageIndex={2}&pageSize={3}", StartTime, EndTime, 1, 1000);
        }
    }
}