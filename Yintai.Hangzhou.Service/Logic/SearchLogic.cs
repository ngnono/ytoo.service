﻿using com.intime.fashion.common;
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

namespace Yintai.Hangzhou.Service.Logic
{
    public static class SearchLogic
    {
        public static void IndexSingle(int id, SourceType type)
        {
            var client = GetClient();
            switch (type)
            {
                case SourceType.Product:
                    try
                    {
                        var items = prepareProducts(id);
                        var response =  client.IndexMany(items);
                        if (!response.IsValid)
                        {
                            foreach(var item in response.Items)
                            {
                                if (!item.OK)
                                    CommonUtil.Log.Error(item.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonUtil.Log.Error(ex);
                    }
                    break;
                default:
                    throw new ArgumentException("type not support");
            }

        }
        public static void IndexSingleAsync(int id, SourceType type)
        {
            Task.Factory.StartNew(() =>
            {
                IndexSingle(id, type);
            });
        }
        private static IEnumerable<ESProduct> prepareProducts(int id)
        {
            var db = ServiceLocator.Current.Resolve<DbContext>();
            var linq = from p in db.Set<ProductEntity>().Where(pe => pe.Id == id).AsQueryable()
                       join s in db.Set<StoreEntity>().AsQueryable() on p.Store_Id equals s.Id
                       join b in db.Set<BrandEntity>().AsQueryable() on p.Brand_Id equals b.Id
                       join t in db.Set<TagEntity>().AsQueryable() on p.Tag_Id equals t.Id
                       let resource = (from r in db.Set<ResourceEntity>().AsQueryable()
                                       where r.SourceId == p.Id
                                       && r.SourceType == 1
                                       select new ESResource()
                                       {
                                           Domain = r.Domain,
                                           Name = r.Name,
                                           SortOrder = r.SortOrder,
                                           IsDefault = r.IsDefault,
                                           Type = r.Type,
                                           Width = r.Width,
                                           Height = r.Height
                                       })
                       let specials = from psp in db.Set<SpecialTopicProductRelationEntity>().AsQueryable()
                                      where psp.Product_Id == p.Id
                                      join sp in db.Set<SpecialTopicEntity>().AsQueryable() on psp.SpecialTopic_Id equals sp.Id
                                      select new ESSpecialTopic
                                      {
                                          Id = sp.Id,
                                          Name = sp.Name,
                                          Description = sp.Description
                                      }
                       let promotions = db.Set<Promotion2ProductEntity>().AsQueryable().Where(pp => pp.ProdId == p.Id)
                                         .Join(db.Set<PromotionEntity>().AsQueryable(), o => o.ProId, i => i.Id, (o, i) => i)
                                         .GroupJoin(db.Set<ResourceEntity>().AsQueryable().Where(pr => pr.SourceType == (int)SourceType.Promotion && pr.Type == (int)ResourceType.Image)
                                                    , o => o.Id
                                                    , i => i.SourceId
                                                    , (o, i) => new { Pro = o, R = i.OrderByDescending(r => r.SortOrder) })
                                         .Select(ppr => new ESPromotion
                                         {
                                             Id = ppr.Pro.Id,
                                             Name = ppr.Pro.Name,
                                             Description = ppr.Pro.Description,
                                             CreatedDate = ppr.Pro.CreatedDate,
                                             StartDate = ppr.Pro.StartDate,
                                             EndDate = ppr.Pro.EndDate,
                                             Status = ppr.Pro.Status,
                                             Resource = ppr.R.Select(r => new ESResource()
                                             {
                                                 Domain = r.Domain,
                                                 Name = r.Name,
                                                 SortOrder = r.SortOrder,
                                                 IsDefault = r.IsDefault,
                                                 Type = r.Type,
                                                 Width = r.Width,
                                                 Height = r.Height
                                             })
                                         })
                       let section = (from section in db.Set<SectionEntity>().AsQueryable()
                                      where section.BrandId == p.Brand_Id && section.StoreId == p.Store_Id
                                      select new ESSection()
                                      {
                                          ContactPerson = section.ContactPerson,
                                          ContactPhone = section.ContactPhone,
                                          Id = section.Id,
                                          Location = section.Location,
                                          Name = section.Name,
                                          Status = section.Status
                                      })

                       let category = (from map in db.Set<ProductMapEntity>() where map.ProductId == p.Id && map.Channel == "intime" select map.ChannelCatId)

                       select new ESProduct()
                       {
                           Id = p.Id,
                           Name = p.Name,
                           Description = p.Description,
                           CreatedDate = p.CreatedDate,
                           Price = p.Price,
                           RecommendedReason = p.RecommendedReason,
                           Status = p.Status,
                           CreateUserId = p.CreatedUser,
                           SortOrder = p.SortOrder,
                           Tag = new ESTag()
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Description = t.Description
                           },
                           Store = new ESStore()
                           {
                               Id = s.Id,
                               Name = s.Name,
                               Description = s.Description,
                               Address = s.Location,
                               Location = new Location
                               {
                                   Lon = s.Longitude,
                                   Lat = s.Latitude
                               },
                               GpsAlt = s.GpsAlt,
                               GpsLat = s.GpsLat,
                               GpsLng = s.GpsLng,
                               Tel = s.Tel
                           },
                           Brand = new ESBrand()
                           {
                               Id = b.Id,
                               Name = b.Name,
                               Description = b.Description,
                               EngName = b.EnglishName
                           },
                           Resource = resource,
                           SpecialTopic = specials,
                           Promotion = promotions,
                           Is4Sale = p.Is4Sale ?? false,
                           UnitPrice = p.UnitPrice,
                           FavoriteCount = p.FavoriteCount,
                           InvolvedCount = p.InvolvedCount,
                           ShareCount = p.ShareCount,
                           RecommendUserId = p.RecommendUser,
                           Section = section.FirstOrDefault(),
                           UpcCode = p.SkuCode,
                           UpdatedDate = p.UpdatedDate,
                           CategoryId = category.FirstOrDefault() == null ? 0 : (category.FirstOrDefault().HasValue ? category.FirstOrDefault().Value : 0),
                           IsSystem = (!p.ProductType.HasValue) || p.ProductType == (int)ProductType.FromSystem

                       };
            return linq;
        }
        public static ElasticClient GetClient()
        {
            return new ElasticClient(new ConnectionSettings(new Uri(ElasticSearchConfigurationSetting.Current.Host))
                                    .SetDefaultIndex(ElasticSearchConfigurationSetting.Current.Index)
                                    .SetMaximumAsyncConnections(10));
        }
    }
}
