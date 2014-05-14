﻿using System.ComponentModel.Composition;

namespace OPCApp.Infrastructure.REST
{
    [Export(typeof(IRestClientFactory))]
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(string baseAddress, string privateKey, string from)
        {
            return new RestClient(baseAddress, privateKey, from);
        }
    }
}