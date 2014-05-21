
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Job.Order.OrderStatusSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intime.OPC.Job.Tests.Order
{
      [TestClass]
    public class OrderTest
    {
        [TestMethod]
        public void OrderSync()
      {
          //var job = new OrderNotifyJob();
          //job.Execute(null);

          var rmaJob = new Intime.OPC.Job.RMASync.RMANotifyJob();
          rmaJob.Execute(null);

          var statusJob = new SaleOrderStatusSyncJob();
        statusJob.Execute(null);
      }
    }
}
