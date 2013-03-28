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
            return DateTime.Now.AddMinutes(-5);
        }
    }
}
