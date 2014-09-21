using System;
using com.intime.o2o.data.exchange.Tmall.Product.Mappers;

namespace com.intime.o2o.data.exchange.Tmall.Mappers.Support
{
    public class DefaultItemMapper : IItemMapper
    {
        public long? ToChannel(int? innerId)
        {
            throw new NotImplementedException();
        }

        public int? FromChannel(long? outerId)
        {
            throw new NotImplementedException();
        }

        public void Save(int? innerId, long? outerId)
        {
            throw new NotImplementedException();
        }
    }
}
