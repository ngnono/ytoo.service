using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Intime.OPC.Modules.Dimension.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.REST;
using OPCAPP.Common.Extensions;
using OPCApp.Infrastructure.Rest;
using System.Threading.Tasks;

namespace Intime.OPC.Modules.Dimension.Common
{
    public abstract class DimensionService<TDimension> : IService<TDimension>, IPartImportsSatisfiedNotification
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension
    {
        private IRestClient restClient;
        private string uriName;

        [Import]
        public TokenManager TokenManager { get; set; }

        [Import]
        private IRestClientFactory RestClientFactory { get; set; }


        public virtual void Initialize()
        {
            restClient = RestClientFactory.Create(AppEx.Config.ServiceUrl, AppEx.Config.UserKey, AppEx.Config.Password);
            restClient.Token = TokenManager.Token;

            var attribute = typeof(TDimension).GetCustomAttribute<UriAttribute>();
            if (attribute == null) throw new InvalidOperationException("Uriattribute not found on the given dimension.");

            uriName = attribute.Name;
        }

        public void OnImportsSatisfied()
        {
            Initialize();
        }

        public TDimension Create(TDimension obj)
        {
            return restClient.Post<TDimension>(uriName, obj);
        }

        public TDimension Update(TDimension obj)
        {
            return restClient.Put<TDimension>(string.Format("{0}/{1}", uriName, obj.ID), obj);
        }

        public void Delete(int id)
        {
            restClient.Delete(string.Format("{0}/{1}", uriName, id));
        }

        public TDimension Query(int id)
        {
            return restClient.Get<TDimension>(string.Format("{0}/{1}", uriName, id));
        }

        public IList<TDimension> Query(string name)
        {
            var queryCriteria = new QueryByName { Name = name, PageIndex = 1, PageSize = 200 };
            return Query(queryCriteria);
        }

        public IList<TDimension> QueryAll()
        {
            var queryCriteria = new QueryAll { PageIndex = 1, PageSize = 200 };
            return Query(queryCriteria);
        }

        private IList<TDimension> Query(IQueryCriteria queryCriteria)
        {
            var reponse = restClient.Get<PagedResult<TDimension>>(string.Format("{0}?{1}", uriName, queryCriteria.BuildQueryString()));
            return reponse.Data;
        }
    }
}
