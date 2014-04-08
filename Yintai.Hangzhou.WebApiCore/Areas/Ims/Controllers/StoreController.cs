using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.Customer;
using Yintai.Hangzhou.Contract.DTO.Request.Customer;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Contract.DTO.Response.Customer;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class StoreController : RestfulController
    {
        private IEFRepository<IMS_AssociateEntity> _associateRepo;
        private IEFRepository<IMS_AssociateBrandEntity> _associateBrandRepo;
        private IEFRepository<IMS_AssociateSaleCodeEntity> _associateSaleCodeRepo;
        private IEFRepository<IMS_AssociateItemsEntity> _associateItemRepo;
        private ICustomerRepository _customerRepo;
        private ICustomerDataService _customerService;
        public StoreController(IEFRepository<IMS_AssociateEntity> associateRepo,
            IEFRepository<IMS_AssociateBrandEntity> associateBrandRepo,
            IEFRepository<IMS_AssociateSaleCodeEntity> associateSaleCodeRepo,
            IEFRepository<IMS_AssociateItemsEntity> associateItemRepo,
            ICustomerRepository customerRepo,
            ICustomerDataService customerService)
        {
            _associateRepo = associateRepo;
            _associateBrandRepo = associateBrandRepo;
            _associateSaleCodeRepo = associateSaleCodeRepo;
            _associateItemRepo = associateItemRepo;
            _customerRepo = customerRepo;
            _customerService = customerService;
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult My(int? authuid)
        {
            var linq = Context.Set<UserEntity>().Where(u => u.Id == authuid.Value)
                       .Join(Context.Set<IMS_AssociateEntity>(), o => o.Id, i => i.UserId, (o, i) => i).First();
           
            var store = GetStoreById(linq.Id,authuid.Value);

            return this.RenderSuccess<IMSStoreDetailResponse>(m => m.Data =store);

        }

        [RestfulAuthorize]
        public ActionResult Create(string invite_code, int? authuid)
        {
            if (string.IsNullOrEmpty(invite_code))
                return this.RenderError(r => r.Message = "邀请码错误！");
            var inviteEntity = Context.Set<IMS_InviteCodeEntity>().Where(ii => ii.Code == invite_code && ii.Status == (int)DataStatus.Normal)
                                 .Join(Context.Set<IMS_SectionOperatorEntity>(), o => o.SectionOperatorId, i => i.Id, (o, i) => new { Inv = o, Sec = i })
                                 .FirstOrDefault();
            if (inviteEntity == null)
            {
                return this.RenderError(r => r.Message = "邀请码不存在！");
            }
            if (Convert.ToBoolean(inviteEntity.Inv.IsBinded))
                return this.RenderError(r => r.Message = "邀请码已绑定！");

            var exitUserEntity = Context.Set<UserEntity>().Find(authuid);
            if (exitUserEntity.UserLevel == (int)UserLevel.DaoGou)
                return this.RenderError(r => r.Message = "用户已经开过店了！");
            //steps:
            // 1. read initial info from invite code tables
            // 2. initialize associate tables
            using (var ts = new TransactionScope())
            {
                var initialBrands = Context.Set<IMS_SectionBrandEntity>().Where(isb => isb.SectionId == inviteEntity.Sec.SectionId);
                var initialSaleCodes = Context.Set<IMS_SalesCodeEntity>().Where(isc => isc.SectionId == inviteEntity.Sec.SectionId);
                var sectionEntity = Context.Set<SectionEntity>().Find(inviteEntity.Sec.SectionId);
                //2.1 update user level to daogou
                exitUserEntity.UserLevel = (int)UserLevel.DaoGou;
                exitUserEntity.UpdatedDate = DateTime.Now;
                exitUserEntity.UpdatedUser = exitUserEntity.Id;
                if (string.IsNullOrEmpty(exitUserEntity.Logo))
                    exitUserEntity.Logo = ConfigManager.IMS_DEFAULT_LOGO;
                _customerRepo.Update(exitUserEntity);

                //2.2 create daogou's associate store
                var assocateEntity = _associateRepo.Insert(new IMS_AssociateEntity()
                {
                    CreateDate = DateTime.Now,
                    CreateUser = authuid.Value,
                    OperateRight = inviteEntity.Inv.AuthRight.Value,
                    Status = (int)DataStatus.Normal,
                    TemplateId = ConfigManager.IMS_DEFAULT_TEMPLATE,
                    UserId = authuid.Value,
                    StoreId = sectionEntity.Id
                });
                //2.3 create daogou's brands
                foreach (var brand in initialBrands)
                {
                    _associateBrandRepo.Insert(new IMS_AssociateBrandEntity()
                    {
                        AssociateId = assocateEntity.Id,
                        BrandId = brand.BrandId,
                        CreateDate = DateTime.Now,
                        CreateUser = authuid.Value,
                        Status = (int)DataStatus.Normal,
                        UserId = authuid.Value
                    });
                }
                //2.4 create daogou's sales code
                foreach (var saleCode in initialSaleCodes)
                {
                    _associateSaleCodeRepo.Insert(new IMS_AssociateSaleCodeEntity()
                    {
                        AssociateId = assocateEntity.Id,
                        Code = saleCode.Code,
                        CreateDate = DateTime.Now,
                        CreateUser = authuid.Value,
                        Status = (int)DataStatus.Normal,
                        UserId = authuid.Value

                    });
                }
                //2.5 create daogou's giftcard
                var giftCardEntity = Context.Set<IMS_GiftCardEntity>().Where(igc => igc.Status == (int)DataStatus.Normal).FirstOrDefault();
                if (giftCardEntity != null)
                {
                    _associateItemRepo.Insert(new IMS_AssociateItemsEntity()
                    {
                        AssociateId = assocateEntity.Id,
                        CreateDate = DateTime.Now,
                        CreateUser = authuid.Value,
                        ItemId = giftCardEntity.Id,
                        ItemType = (int)ComboType.GiftCard,
                        Status = (int)DataStatus.Normal,
                        UpdateDate = DateTime.Now,
                        UpdateUser = authuid.Value
                    });
                }
                ts.Complete();
               
            }
            var response = _customerService.GetUserInfo(new GetUserInfoRequest
            {
                AuthUid = authuid.Value
            });
            return this.RenderSuccess<CustomerInfoResponse>(c => c.Data = response.Data);
           
        }

        [RestfulAuthorize]
        public ActionResult Detail(int id, int? authuid)
        {
            var store = GetStoreById(id,authuid.Value);
            return this.RenderSuccess<IMSStoreDetailResponse>(m => m.Data = store);

        }

        private IMSStoreDetailResponse GetStoreById(int storeId,int authuid)
        {
            var linq = Context.Set<UserEntity>()
                       .Join(Context.Set<IMS_AssociateEntity>().Where(ia=>ia.Id == storeId), o => o.Id, i => i.UserId, (o, i) => new { U = o, Store = i }).First();
            var giftCards = Context.Set<IMS_AssociateItemsEntity>().Where(iai => iai.AssociateId == linq.Store.Id
                                             && iai.ItemType == (int)ComboType.GiftCard
                                             && iai.Status == (int)DataStatus.Normal)
                                 .Join(Context.Set<IMS_GiftCardEntity>(), o => o.ItemId, i => i.Id, (o, i) => i)
                                 .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.GiftCard && r.Status == (int)DataStatus.Normal),
                                                 o => o.Id, i => i.SourceId, (o, i) => new { G = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() })
                                 .Select(l => new IMSGiftCard()
                                 {
                                     Id = l.G.Id,
                                     Desc = l.G.Name,
                                     ImageUrl = l.R == null ? string.Empty : l.R.Name
                                 }).ToList();
            var combos = Context.Set<IMS_AssociateItemsEntity>().Where(iai => iai.AssociateId == linq.Store.Id
                                             && iai.ItemType == (int)ComboType.Product
                                             && iai.Status == (int)DataStatus.Normal)
                                 .Join(Context.Set<IMS_ComboEntity>(), o => o.ItemId, i => i.Id, (o, i) => i)
                                 .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Combo && r.Status == (int)DataStatus.Normal),
                                                 o => o.Id, i => i.SourceId, (o, i) => new { C = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() })
                                 .GroupJoin(Context.Set<IMS_Combo2ProductEntity>()
                                             .GroupJoin(Context.Set<ResourceEntity>()
                                                         .Where(r => r.SourceType == (int)SourceType.Product && r.Status == (int)DataStatus.Normal && r.Type == (int)ResourceType.Image)
                                                             , o => o.ProductId,
                                                             i => i.SourceId,
                                                             (o, i) => new { P = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() })
                                          , o => o.C.Id
                                          , i => i.P.ComboId
                                          , (o, i) => new { C = o.C, CR = o.R, P = i })
                                  .Select(l => new IMSCombo()
                                  {
                                      Id = l.C.Id,
                                      Desc = l.C.Desc,
                                      ImageUrl = l.CR.Name,
                                      Price = l.C.Price,
                                      ProductImageUrls = l.P.Select(lpr => lpr.R.Name)
                                  });
            return new IMSStoreDetailResponse()
            {
                Id = linq.Store.Id,
                Name = linq.U.Nickname,
                Logo = linq.U.Logo,
                GiftCardSaling = giftCards,
                ComboSaling = combos,
                Mobile = linq.U.Mobile,
                Is_Owner = authuid == linq.U.Id,
                Is_Favored = Context.Set<FavoriteEntity>().Any(f => f.User_Id == authuid &&
                                    f.FavoriteSourceType == (int)SourceType.Store &&
                                    f.FavoriteSourceId == storeId &&
                                    f.Status == (int)DataStatus.Normal)
            };
        }
    }
}
