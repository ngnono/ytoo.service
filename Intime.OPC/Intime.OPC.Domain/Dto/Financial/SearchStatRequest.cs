using System;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Dto.Financial
{
    public class SearchStatRequest 
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void FormatDate()
        {
            StartTime = StartTime.Date;
            EndTime = EndTime.Date.AddDays(1);
        }
    }
}