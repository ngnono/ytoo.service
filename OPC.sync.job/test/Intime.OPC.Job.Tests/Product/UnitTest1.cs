using System;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intime.OPC.Job.Tests.Product
{
    [TestClass]
    public class AllSyncJobTest
    {
        [TestMethod]
        public void SyncTest()
        {
            var job = new AllSyncJob();
            job.Execute(null);
        }
    }
}
