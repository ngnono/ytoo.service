using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.jobscheduler.Job
{
    public static class JobConfig
    {
        public static readonly int DEFAULT_PAGE_SIZE = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
    }
}
