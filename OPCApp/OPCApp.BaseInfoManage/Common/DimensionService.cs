using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Intime.OPC.Modules.Dimension.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.REST;
using OPCAPP.Common.Extensions;

namespace Intime.OPC.Modules.Dimension.Common
{
    public abstract class DimensionService<TDimension> : IService<TDimension>
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension
    {
        private IRestClient restClient;
        private string uriName;

        [Import]
        public IRestClientFactory RestClientFactory { get; set; }

        public virtual void Initialize()
        {
            this.restClient = RestClientFactory.Create(AppEx.Config.ServiceUrl, AppEx.Config.UserKey, AppEx.Config.Password);
            var attribute = typeof(TDimension).GetCustomAttribute<UriAttribute>();
            if (attribute == null) throw new InvalidOperationException("Uriattribute not found on the given dimension.");

            uriName = attribute.Name;
        }

        public TDimension Create(TDimension obj)
        {
            var request = new Request<TDimension>()
            {
                Data = obj,
                URI = uriName
            };
            var response = restClient.Post<TDimension, Response<TDimension>>(request);
            return response.Data;
        }

        public void Update(TDimension obj)
        {
            var request = new Request<TDimension>()
            {
                Data = obj,
                URI = string.Format("{0}/{1}",uriName, obj.ID)
            };
            restClient.Put<TDimension, Response<TDimension>>(request);
        }

        public void Delete(int id)
        {
            restClient.Delete<Response<Counter>>(string.Format("{0}/{1}", uriName, id));
        }

        public TDimension Query(int id)
        {
            var response = restClient.Get<Response<TDimension>>(string.Format("{0}/{1}", uriName, id));
            return response.Data;
        }

        public IList<TDimension> Query(string name)
        {
            var response = restClient.Get<Response<IList<TDimension>>>(string.Format("{0}?nameprefix={1}",uriName, name));
            return response.Data;
        }

        public IList<TDimension> QueryAll()
        {
            var response = restClient.Get<Response<IList<TDimension>>>(uriName);
            return response.Data;
        }
    }
}
