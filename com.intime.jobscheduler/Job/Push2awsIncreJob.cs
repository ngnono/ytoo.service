using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler.Job
{
    class Push2awsIncreJob:Push2awsJob
    {
        protected override DateTime BenchDate(Quartz.IJobExecutionContext context)
        {
            //every 5mins, figure out new create/updated products, promotions
            var dataMap = context.JobDetail.JobDataMap;
            int intervalMins =  dataMap.ContainsKey("interval") ? dataMap.GetIntValue("interval") : 5;
            return DateTime.Now.AddMinutes(-intervalMins);
        }
        protected override bool CascadPush
        {
            get
            {
                return true;
            }
        }
    }
}
