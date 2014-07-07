using CLAP;
using com.intime.fashion.service.search;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.console.onetime
{
    partial class OneTimeCommand
    {
        [Verb(IsDefault = false, Description = "reindex types", Aliases = "reindex_type")]
        static void Reindex_Types(
            [Aliases("type")]
            [Description("p:product,b:brand")]
            [Required]
            string type)
        {
            var indexType = IndexSourceType.Product;
            IEnumerable<int> idList = null;
            if (type == "p")
            {
                indexType = IndexSourceType.Product;
                idList = allActiveProducts();
            }
            else if (type == "b")
            {
                indexType = IndexSourceType.Brand;
                idList = allActiveBrands();
            }
            else
            {
                Console.WriteLine("not support type");
                return;
            }

            var indexService = SearchLogic.GetService(indexType);
            
            
            foreach (var iid in idList)
            {
                indexService.IndexSingle(iid);
                Console.WriteLine(string.Format("{0} indexed", iid));
            }
        }

        private static IEnumerable<int> allActiveBrands()
        {
            var db = ServiceLocator.Current.Resolve<DbContext>();
            return db.Set<BrandEntity>().Where(p => p.Status == (int)DataStatus.Normal)
                   .Select(p => p.Id).ToList();
        }

        private static IEnumerable<int> allActiveProducts()
        {
            var db = ServiceLocator.Current.Resolve<DbContext>();
            return db.Set<ProductEntity>().Where(p => p.Status == (int)DataStatus.Normal)
                   .Select(p => p.Id).ToList();
               
        }
    }
}
