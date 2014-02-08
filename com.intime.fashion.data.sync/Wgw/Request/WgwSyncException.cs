using System;

namespace com.intime.jobscheduler.Job.Wgw
{
    public class WgwSyncException:Exception
    {
        public WgwSyncException(string msg) : base(msg)
        {
        }
    }
}
