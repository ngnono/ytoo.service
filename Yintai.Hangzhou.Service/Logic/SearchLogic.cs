using com.intime.fashion.common;
using com.intime.fashion.common.config;
using Nest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.Service.Logic.Search;

namespace Yintai.Hangzhou.Service.Logic
{
    public static class SearchLogic
    {
        private static ElasticClient _client = new ElasticClient(new ConnectionSettings(new Uri(ElasticSearchConfigurationSetting.Current.Host))
                                        .SetDefaultIndex(ElasticSearchConfigurationSetting.Current.Index)
                                        .SetMaximumAsyncConnections(10));
        
        public static void IndexSingle(int id, SourceType type)
        {
            var serviceBase = GetService(type);
            serviceBase.IndexSingle(id);
           
        }
        public static bool IndexSingle<T>(T source) where T : class
        {
            var client = GetClient();
            var response = client.Index<T>(source);
            if (!response.IsValid)
            {
                var error = response.ConnectionStatus;
                if (error == null)
                    CommonUtil.Log.Error(response);
                else
                    CommonUtil.Log.Error(error.ToString());
                return false;
            }
           
            return true;

        }
        public static void IndexSingleAsync(int id, SourceType type)
        {
            Task.Factory.StartNew(() =>
            {
                IndexSingle(id, type);
            });
        }
        public static ESServiceBase GetService(SourceType type)
        {
            switch (type)
            { 
                case SourceType.Combo:
                    return new ESComboService();
                case SourceType.Product:
                    return new ESProductService();
                default:
                    throw new ArgumentException("type mismatch");
            }
        }
   
        public static ElasticClient GetClient()
        {
            return _client;
        }

      
    }
}
