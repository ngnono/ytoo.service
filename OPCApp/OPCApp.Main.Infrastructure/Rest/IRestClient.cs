using System;

namespace OPCApp.Infrastructure.REST
{
    /// <summary>
    /// Api接口类
    /// </summary>
    public interface IRestClient
    {
        TResponse Get<TResponse>(string uri);

        TResponse Post<TData, TResponse>(Request<TData> request);

        TResponse Put<TData, TResponse>(Request<TData> request);

        TResponse Delete<TResponse>(string uri);
    }
}
