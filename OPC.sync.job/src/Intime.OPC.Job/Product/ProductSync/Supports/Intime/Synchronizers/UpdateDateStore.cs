using System;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers
{
    public class UpdateDateStore : IUpdateDateStore
    {
        public DateTime GetLast(string key)
        {
            //TODO:修改时间
            return new DateTime(2013, 11, 15);
        }

        public void Update(string key, DateTime dateTime)
        {
            //TODO:Save DateTime
        }
    }
}
