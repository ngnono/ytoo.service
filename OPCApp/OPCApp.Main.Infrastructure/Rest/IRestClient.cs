using System;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.REST
{
    /// <summary>
    /// Api接口类
    /// </summary>
    public interface IRestClient
    {
        string Token { get; set; }

        TData Get<TData>(string uri);

        TEntity Post<TEntity>(string uri, TEntity entity);

        TEntity Put<TEntity>(string uri, TEntity entity);

        void Delete(string uri);
    }
}
