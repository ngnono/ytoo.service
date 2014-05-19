using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.REST;
using OPCAPP.Common.Extensions;
using OPCApp.Infrastructure.Rest;
using Intime.OPC.Infrastructure.Service;

namespace Intime.OPC.Modules.Dimension.Common
{
    public abstract class DimensionService<TDimension> : IService<TDimension>, IPartImportsSatisfiedNotification
        where TDimension : OPCApp.Domain.Models.Dimension
    {
        private IRestClient _restClient;
        private string _uriName;

        [Import]
        public TokenManager TokenManager { get; set; }

        [Import]
        private IRestClientFactory RestClientFactory { get; set; }


        public virtual void Initialize()
        {
            _restClient = RestClientFactory.Create(AppEx.Config.ServiceUrl, AppEx.Config.UserKey, AppEx.Config.Password);
            _restClient.Token = TokenManager.Token;

            var attribute = typeof(TDimension).GetCustomAttribute<UriAttribute>();
            if (attribute == null) throw new InvalidOperationException("Uriattribute not found on the given dimension.");

            _uriName = attribute.Name;
        }

        public void OnImportsSatisfied()
        {
            Initialize();
        }

        public TDimension Create(TDimension obj)
        {
            return _restClient.Post<TDimension>(_uriName, obj);
        }

        public TDimension Update(TDimension obj)
        {
            return _restClient.Put<TDimension>(string.Format("{0}/{1}", _uriName, obj.ID), obj);
        }

        public void Delete(int id)
        {
            _restClient.Delete(string.Format("{0}/{1}", _uriName, id));
        }

        public TDimension Query(int id)
        {
            return _restClient.Get<TDimension>(string.Format("{0}/{1}", _uriName, id));
        }

        public PagedResult<TDimension> Query(IQueryCriteria queryCriteria)
        {
            return _restClient.Get<PagedResult<TDimension>>(string.Format("{0}?{1}", _uriName, queryCriteria.BuildQueryString()));
        }
    }
}
