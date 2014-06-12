
using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace com.intime.fashion.service
{
    public static class ComboLogic
    {
        public static bool IfCanOnline(int userId)
        {
            var onlineCount = Context.Set<IMS_AssociateItemsEntity>().Where(ia=>ia.Status==(int)DataStatus.Normal && 
                                    ia.ItemType==(int)ComboType.Product)
                               .Join(Context.Set<IMS_ComboEntity>().Where(ic=>ic.ExpireDate>DateTime.Now),o=>o.ItemId,i=>i.Id,(o,i)=>o)
                               .Join(Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == userId), o => o.AssociateId, i => i.Id,
                                (o, i) => o).Count();
            return onlineCount < ConfigManager.MAX_COMBO_ONLINE;
        }
        private static DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }

        public static void OfflineComboOne(int authuid)
        {
            //use the LFLO policy to offline the oldest combo
            var associateItemRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateItemsEntity>>();
            var comboRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_ComboEntity>>();
            var associateItemEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                               .Join(Context.Set<IMS_AssociateItemsEntity>().Where(iai => iai.ItemType == (int)ComboType.Product && iai.Status==(int)DataStatus.Normal), o => o.Id, i => i.AssociateId, (o, i) => i)
                               .OrderBy(ia=>ia.UpdateDate)
                               .FirstOrDefault();
            associateItemEntity.Status = (int)DataStatus.Default;
            associateItemEntity.UpdateDate = DateTime.Now;
            associateItemRepo.Update(associateItemEntity);

            var comboEntity = Context.Set<IMS_ComboEntity>().Find(associateItemEntity.ItemId);
            comboEntity.Status = (int)DataStatus.Default;
            comboEntity.UpdateDate = DateTime.Now;
            comboRepo.Update(comboEntity);
        }

        public static IMS_ComboEntity CreateComboFromProduct(ProductEntity productEntity, IMS_AssociateEntity associateEntity)
        {
            var associateItemRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateItemsEntity>>();
            var comboRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_ComboEntity>>();
            var combo2productRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_Combo2ProductEntity>>();
            var resourceRepo = ServiceLocator.Current.Resolve<IResourceRepository>();
         
            var createUserId = associateEntity.UserId;
            //step1: offline one other combo
            if (!ComboLogic.IfCanOnline(createUserId))
            {
                ComboLogic.OfflineComboOne(createUserId);
            }
            //step1.1: create combo
            var comboEntity = comboRepo.Insert(new IMS_ComboEntity()
            {
                CreateDate = DateTime.Now,
                CreateUser = createUserId,
                Desc = productEntity.Description,
                OnlineDate = DateTime.Now,
                Price = productEntity.Price,
                Private2Name = string.Empty,
                Status = (int)DataStatus.Normal,
                UpdateDate = DateTime.Now,
                UpdateUser = createUserId,
                UserId = createUserId,
                ProductType = (int)ProductType.FromSelf,
                ExpireDate = DateTime.Now.AddDays(ConfigManager.COMBO_EXPIRED_DAYS)
            });

            //step2: create combo2product
            combo2productRepo.Insert(new IMS_Combo2ProductEntity()
                {
                    ComboId = comboEntity.Id,
                    ProductId = productEntity.Id
                });


            //step2.1 associate combo
            associateItemRepo.Insert(new IMS_AssociateItemsEntity()
            {
                AssociateId = associateEntity.Id,
                CreateDate = DateTime.Now,
                CreateUser = createUserId,
                ItemId = comboEntity.Id,
                ItemType = (int)ComboType.Product,
                Status = (int)DataStatus.Normal,
                UpdateDate = DateTime.Now,
                UpdateUser = createUserId
            });

            //step3: bind images

            foreach (var resource in Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product &&
                                            r.SourceId == productEntity.Id &&
                                            r.Status == (int)DataStatus.Normal &&
                                            r.Type == (int)ResourceType.Image))
            {
                resourceRepo.Insert(new ResourceEntity()
                {
                    Size = resource.Size,
                    SortOrder = resource.SortOrder,
                    Name = resource.Name,
                    Height = resource.Height,
                    ExtName = resource.ExtName,
                    Domain = resource.Domain,
                    CreatedUser = resource.CreatedUser,
                    CreatedDate = DateTime.Now,
                    ContentSize = resource.ContentSize,
                    SourceId = comboEntity.Id,
                    SourceType = (int)SourceType.Combo,
                    Type = resource.Type,
                    Status = resource.Status,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = resource.UpdatedUser,
                    Width = resource.Width
                });
            }

          
            return comboEntity;
        }
    }
}
