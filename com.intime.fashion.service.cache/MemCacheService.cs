using com.intime.fashion.common.config;
using com.intime.fashion.service.contract;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.intime.fashion.service.cache
{
   public class MemCacheService:ICacheService
   {
       public MemcachedClient _client = null;
       private string _prefix = string.Empty;

        public IEnumerable<T> GetList<T>(string name, Func<IEnumerable<T>> refreshResult)
        {
            EnsureClient();
            object items = null;
            if (!_client.TryGet(formatKey(name), out items))
            {
                items = refreshResult().ToArray();
                _client.Store(StoreMode.Set, formatKey(name), items);

            }
            return items as IEnumerable<T>;
        }
        public T Get<T>(string name, Func<T> refreshResult) where T: class
        {
            EnsureClient();
            object item = null;
            if (!_client.TryGet(formatKey(name), out item))
            {
                item = refreshResult();
                _client.Store(StoreMode.Set, formatKey(name), item);

            }
            return item as T;
        }
        private string formatKey(string name)
        {
            return string.Concat(_prefix, name);
        }

        private void EnsureClient()
        {
            if (_client == null)
            {
                MemcachedClientConfiguration config = new MemcachedClientConfiguration();
                var memConfig = CommonConfiguration<Cache_AuthkeyConfiguration>.Current;
                config.AddServer(memConfig.Host,int.Parse(memConfig.Port));
                config.Protocol = MemcachedProtocol.Binary;
                config.Authentication.Type = typeof(PlainTextAuthenticator);
                config.Authentication.Parameters["userName"] = memConfig.UserName;
                config.Authentication.Parameters["password"] = memConfig.Password;
                _prefix = memConfig.Prefix;
                _client = new MemcachedClient(config);
            }
        }

       
   }
}
