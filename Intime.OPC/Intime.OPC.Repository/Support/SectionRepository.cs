// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 01:21:46
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 01:22:26
// ***********************************************************************
// <copyright file="SectionRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;
using PredicateBuilder = LinqKit.PredicateBuilder;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///  专柜
    /// </summary>
    public class SectionRepository : OPCBaseRepository<int, Section>, ISectionRepository
    {
        #region methods

        private static Expression<Func<Section, bool>> Filler(SectionFilter filter)
        {
            var query = PredicateBuilder.True<Section>();

            if (filter != null)
            {
                if (!String.IsNullOrWhiteSpace(filter.NamePrefix))
                    query = PredicateBuilder.And(query, v => v.Name.StartsWith(filter.NamePrefix));

                if (filter.Status != null)
                {
                    query = PredicateBuilder.And(query, v => v.Status == filter.Status);
                }

                if (!String.IsNullOrWhiteSpace(filter.Name))
                {
                    query = PredicateBuilder.And(query, v => v.Name == filter.Name);
                }
            }

            return query;
        }

        private static Expression<Func<IMS_SectionBrand, bool>> Filler4Brand(SectionFilter filter)
        {
            var query = PredicateBuilder.True<IMS_SectionBrand>();

            if (filter != null)
            {
                if (filter.BrandId != null)
                {
                    query = PredicateBuilder.And(query, v => v.BrandId == filter.BrandId);
                }

            }

            return query;

        }

        private static Func<IQueryable<Section>, IOrderedQueryable<Section>> OrderBy(SectionSortOrder sortOrder)
        {
            Func<IQueryable<Section>, IOrderedQueryable<Section>> orderBy = null;

            switch (sortOrder)
            {
                default:
                    orderBy = v => v.OrderByDescending(s => s.Id);
                    break;
            }

            return orderBy;
        }

        /// <summary>
        /// 关系处理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="entity"></param>
        private void Relationship(DbContext c, Section entity, List<int> brandids)
        {
            EFHelper.Delete<IMS_SectionBrand>(c, v => v.SectionId == entity.Id);

            if (brandids != null)
            {
                var list =
                    brandids.Select(v => new IMS_SectionBrand() { SectionId = entity.Id, BrandId = v });

                c.Set<IMS_SectionBrand>().AddOrUpdate(v =>
                 new
                 {
                     v.BrandId,
                     v.SectionId
                 }
                , list.ToArray());

                c.SaveChanges();
                //EFHelper.InsertOrUpdate(c, list.ToArray());
            }
        }


        #endregion

        public override IEnumerable<Section> AutoComplete(string query)
        {
            var filter = Filler(new SectionFilter { NamePrefix = query });
            return Func(v => EFHelper.Get(DbQuery(v), filter, null).ToList());
        }

        public List<Section> GetPagedList(PagerRequest pagerRequest, out int totalCount, SectionFilter filter, SectionSortOrder sortOrder)
        {
            var brandFilter = Filler4Brand(filter);
            var result = Func(c =>
            {
                int t;

                var q1 = from section in c.Set<Section>().Where(Filler(filter))
                         select new { section };

                var qq = from sec_brand in c.Set<IMS_SectionBrand>().Where(brandFilter)
                         select new { sec_brand };

                if (filter.BrandId == null)
                {
                    var qt = from s in q1
                             join s_b in qq on s.section.Id equals
                        s_b.sec_brand.SectionId into tmp1
                             from s_b in tmp1.DefaultIfEmpty()
                             select new
                             {
                                 s.section,
                                 s_b.sec_brand.BrandId
                             };

                    t = qt.Count();

                    var qr = from s in qt.OrderBy(v => v.section.Id).Skip(pagerRequest.SkipCount).Take(pagerRequest.PageSize)
                             let brands = (
                                 from b in c.Set<Brand>()
                                 where s.BrandId == b.Id
                                 select new BrandClone
                                 {
                                     ChannelBrandId = b.ChannelBrandId,
                                     CreatedDate = b.CreatedDate,
                                     CreatedUser = b.CreatedUser,
                                     Description = b.Description,
                                     EnglishName = b.EnglishName,
                                     Group = b.Group,
                                     Id = b.Id,
                                     Logo = b.Logo,
                                     Name = b.Name,
                                     Status = b.Status,
                                     UpdatedDate = b.UpdatedDate,
                                     UpdatedUser = b.UpdatedUser,
                                     WebSite = b.WebSite
                                 }
                                 )
                             select new SectionClone()
                             {
                                 BrandId = s.section.BrandId,
                                 ChannelSectionId = s.section.ChannelSectionId,
                                 ContactPerson = s.section.ContactPerson,
                                 ContactPhone = s.section.ContactPhone,
                                 CreateDate = s.section.CreateDate,
                                 CreateUser = s.section.CreateUser,
                                 Id = s.section.Id,
                                 Location = s.section.Location,
                                 Name = s.section.Name,
                                 Status = s.section.Status,
                                 StoreCode = s.section.StoreCode,
                                 StoreId = s.section.StoreId,
                                 UpdateDate = s.section.UpdateDate,
                                 UpdateUser = s.section.UpdateUser,
                                 Brands = brands
                             };


                    return new
                    {
                        totalCount = t,
                        Data = qr.ToList()
                    };
                }
                else
                {
                    var qt = from s in q1
                             join s_b in qq on s.section.Id equals
                        s_b.sec_brand.SectionId
                             select new
                             {
                                 s.section,
                                 s_b.sec_brand.BrandId
                             };
                    t = qt.Count();

                    var qr = from s in qt.OrderBy(v => v.section.Id).Skip(pagerRequest.SkipCount).Take(pagerRequest.PageSize)
                             let brands = (
                                 from b in c.Set<Brand>()
                                 where s.BrandId == b.Id
                                 select new BrandClone
                                 {
                                     ChannelBrandId = b.ChannelBrandId,
                                     CreatedDate = b.CreatedDate,
                                     CreatedUser = b.CreatedUser,
                                     Description = b.Description,
                                     EnglishName = b.EnglishName,
                                     Group = b.Group,
                                     Id = b.Id,
                                     Logo = b.Logo,
                                     Name = b.Name,
                                     Status = b.Status,
                                     UpdatedDate = b.UpdatedDate,
                                     UpdatedUser = b.UpdatedUser,
                                     WebSite = b.WebSite
                                 }
                                 )
                             select new SectionClone()
                             {
                                 BrandId = s.section.BrandId,
                                 ChannelSectionId = s.section.ChannelSectionId,
                                 ContactPerson = s.section.ContactPerson,
                                 ContactPhone = s.section.ContactPhone,
                                 CreateDate = s.section.CreateDate,
                                 CreateUser = s.section.CreateUser,
                                 Id = s.section.Id,
                                 Location = s.section.Location,
                                 Name = s.section.Name,
                                 Status = s.section.Status,
                                 StoreCode = s.section.StoreCode,
                                 StoreId = s.section.StoreId,
                                 UpdateDate = s.section.UpdateDate,
                                 UpdateUser = s.section.UpdateUser,
                                 Brands = brands
                             };

                    return new
                    {
                        totalCount = t,
                        Data = qr.ToList()
                    };
                }

            });

            totalCount = result.totalCount;

            return AutoMapper.Mapper.Map<List<SectionClone>, List<Section>>(result.Data);
        }


        public override Section GetItem(int key)
        {
            return Func(c =>
            {
                var q1 = from section in c.Set<Section>().Where(v => v.Id == key)
                         select new { section };

                var qq = from sec_brand in c.Set<IMS_SectionBrand>()
                         select new { sec_brand };


                var qt = from s in q1
                         join s_b in qq on s.section.Id equals
                    s_b.sec_brand.SectionId into tmp1
                         from s_b in tmp1.DefaultIfEmpty()
                         select new
                         {
                             s.section,
                             s_b.sec_brand.BrandId
                         };
                var qr = from s in qt
                         let brands = (
                             from b in c.Set<Brand>()
                             where s.BrandId == b.Id
                             select new BrandClone
                             {
                                 ChannelBrandId = b.ChannelBrandId,
                                 CreatedDate = b.CreatedDate,
                                 CreatedUser = b.CreatedUser,
                                 Description = b.Description,
                                 EnglishName = b.EnglishName,
                                 Group = b.Group,
                                 Id = b.Id,
                                 Logo = b.Logo,
                                 Name = b.Name,
                                 Status = b.Status,
                                 UpdatedDate = b.UpdatedDate,
                                 UpdatedUser = b.UpdatedUser,
                                 WebSite = b.WebSite
                             }
                             )
                         select new SectionClone()
                         {
                             BrandId = s.section.BrandId,
                             ChannelSectionId = s.section.ChannelSectionId,
                             ContactPerson = s.section.ContactPerson,
                             ContactPhone = s.section.ContactPhone,
                             CreateDate = s.section.CreateDate,
                             CreateUser = s.section.CreateUser,
                             Id = s.section.Id,
                             Location = s.section.Location,
                             Name = s.section.Name,
                             Status = s.section.Status,
                             StoreCode = s.section.StoreCode,
                             StoreId = s.section.StoreId,
                             UpdateDate = s.section.UpdateDate,
                             UpdateUser = s.section.UpdateUser,
                             Brands = brands
                         };




                return AutoMapper.Mapper.Map<SectionClone, Section>(qr.FirstOrDefault());


            });
        }

        public IList<Section> GetByStoreIDs(IList<int> storeIDs)
        {
            return Select(t => t.StoreId.HasValue && storeIDs.Contains(t.StoreId.Value));
        }

        public override Section Insert(Section entity)
        {
            return Func(c =>
            {
                List<int> brandids = null;
                if (entity.Brands != null)
                {
                    brandids = entity.Brands.Select(v => v.Id).ToList();
                    entity.Brands = null;
                }

                var item = EFHelper.Insert(c, entity);
                entity.Id = item.Id;


                Relationship(c, entity, brandids);

                return item;
            });
        }

        public override void Update(Section entity)
        {
            Action(c =>
            {

                List<int> brandids = null;
                if (entity.Brands != null)
                {
                    brandids = entity.Brands.Select(v => v.Id).ToList();
                    entity.Brands = null;
                }



                EFHelper.Update(c, entity);

                Relationship(c, entity, brandids);
            });
        }

        public override void Delete(int id)
        {
            base.Delete(id);
        }
    }
}