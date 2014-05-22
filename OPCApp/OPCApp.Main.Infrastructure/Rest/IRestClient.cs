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

        #region CRUD

        TData Get<TData>(string uri);

        TEntity Post<TEntity>(string uri, TEntity entity);

        TEntity Put<TEntity>(string uri, TEntity entity);

        void Delete(string uri);

        #endregion

        TEntity Post<TEntity, TData>(string uri, TData data);

        TEntity Put<TEntity, TData>(string uri, TData data);

        void PutWithoutReturn(string uri);

        void PutWithoutReturn<TEntity>(string uri, TEntity entity);

        void PostWithoutReturnValue<TEntity>(string uri, TEntity entity);
    }
}
