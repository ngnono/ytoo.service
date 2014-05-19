using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Rest;
using OPCApp.Infrastructure.REST;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCAPP.Common.Extensions;
using OPCApp.Domain.Attributes;

namespace Intime.OPC.Infrastructure.Service
{
    public class ServiceBase<TModel> : IService<TModel>, IPartImportsSatisfiedNotification
        where TModel : OPCApp.Domain.Models.Model
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

            var attribute = typeof(TModel).GetCustomAttribute<UriAttribute>();
            if (attribute == null) throw new InvalidOperationException("Uriattribute not found on the given dimension.");

            _uriName = attribute.Name;
        }

        public void OnImportsSatisfied()
        {
            Initialize();
        }

        public virtual TModel Create(TModel obj)
        {
            return _restClient.Post<TModel>(_uriName, obj);
        }

        public virtual TModel Update(TModel obj)
        {
            return _restClient.Put<TModel>(string.Format("{0}/{1}", _uriName, obj.Id), obj);
        }

        public virtual void Delete(int id)
        {
            _restClient.Delete(string.Format("{0}/{1}", _uriName, id));
        }

        public virtual TModel Query(int id)
        {
            return _restClient.Get<TModel>(string.Format("{0}/{1}", _uriName, id));
        }

        public virtual PagedResult<TModel> Query(IQueryCriteria queryCriteria)
        {
            return _restClient.Get<PagedResult<TModel>>(string.Format("{0}?{1}", _uriName, queryCriteria.BuildQueryString()));
        }

        public IList<TModel> QueryAll(IQueryCriteria queryCriteria)
        {
            var pagedResult = _restClient.Get<PagedResult<TModel>>(string.Format("{0}?{1}", _uriName, queryCriteria.BuildQueryString()));
            if (pagedResult.TotalCount == pagedResult.Data.Count) return pagedResult.Data;

            var result = new List<TModel>(pagedResult.Data);
            var pages = pagedResult.TotalCount / pagedResult.PageSize + (pagedResult.TotalCount % pagedResult.PageSize == 0 ? 0: 1);

            for (int i = pagedResult.PageIndex; i < pages; i++)
            {
                queryCriteria.PageIndex++;
                pagedResult = _restClient.Get<PagedResult<TModel>>(string.Format("{0}?{1}", _uriName, queryCriteria.BuildQueryString()));
                result.AddRange(pagedResult.Data);
            }

            return result;
        }
    }
}
