using System;

namespace Intime.OPC.Job.Product.ProductSync.Supports.Intime.Synchronizers
{
    public class UpdateDateStore : IUpdateDateStore
    {
        public DateTime GetLast(string key)
        {
            //TODO:修改时间
            return DateTime.Now.AddHours(-2);
        }

        public void Update(string key, DateTime dateTime)
        {
            //TODO:Save DateTime
        }
    }
}
