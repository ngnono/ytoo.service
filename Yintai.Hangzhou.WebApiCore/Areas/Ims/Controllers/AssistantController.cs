﻿using com.intime.fashion.common;
using com.intime.fashion.common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class AssistantController : RestfulController
    {
        private IEFRepository<IMS_AssociateSaleCodeEntity> _salescodeRepo;
        private IEFRepository<IMS_AssociateItemsEntity> _associateitemRepo;
        private IEFRepository<IMS_AssociateIncomeRequestEntity> _incomerequestRepo;
        private ICustomerRepository _userRepo;
        private IEFRepository<IMS_AssociateEntity> _associateRepo;
        private IFeedbackRepository _feedbackRepo;
        private IEFRepository<IMS_GiftCardEntity> _cardRepo;
        private IResourceRepository _resourceRepo;
        private IInventoryRepository _inventoryRepo;
        public AssistantController(IEFRepository<IMS_AssociateSaleCodeEntity> salescodeRepo,
            IEFRepository<IMS_AssociateItemsEntity> associateitemRepo,
            IEFRepository<IMS_AssociateIncomeRequestEntity> incomerequestRepo,
            ICustomerRepository userRepo,
            IEFRepository<IMS_AssociateEntity> associateRepo,
            IFeedbackRepository feedbackRepo,
            IEFRepository<IMS_GiftCardEntity> cardRepo,
            IResourceRepository resourceRepo,
            IInventoryRepository inventoryRepo
            )
        {
            _salescodeRepo = salescodeRepo;
            _associateitemRepo = associateitemRepo;
            _incomerequestRepo = incomerequestRepo;
            _userRepo = userRepo;
            _associateRepo = associateRepo;
            _feedbackRepo = feedbackRepo;
            _cardRepo = cardRepo;
            _resourceRepo = resourceRepo;
            _inventoryRepo = inventoryRepo;
        }
        [RestfulAuthorize]
        public ActionResult Gift_Cards(PagerInfoRequest request)
        {
            int page = request.Page <= 0 ? 0 : request.Page - 1;
            int pagesize = request.Pagesize >= 40 || request.Pagesize <= 0 ? 20 : request.Pagesize;
            var count = _cardRepo.Get(x => x.Status == 1).Count();
            var cards = new List<dynamic>();     
            var linq =
                _cardRepo.Get(x => x.Status == 1)
                    .OrderByDescending(x => x.UpdateDate)
                    .Skip(page*pagesize)
                    .Take(pagesize)
                    .GroupJoin(_resourceRepo.Get(x => x.SourceType == (int) SourceType.GiftCard), c => c.Id,
                        s => s.SourceId, (c, rs) => new {card = c, image= rs.FirstOrDefault()});
            foreach (var cr in linq)
            {
                cards.Add(new
                {
                    id = cr.card.Id,
                    desc = cr.card.Name,
                    image = cr.image.Name.Image320Url(),
                    is_online = true,
                });
            }
            var rsp = new PagerInfoResponse<dynamic>(request.PagerRequest, count) {Items = cards};
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = rsp);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Combos(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                       .Join(Context.Set<IMS_AssociateItemsEntity>().Where(ia => ia.ItemType == (int)ComboType.Product), o => o.Id, i => i.AssociateId, (o, i) => i)
                       .Join(Context.Set<IMS_ComboEntity>(), o => o.ItemId, i => i.Id, (o, i) => i)
                       .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Combo && r.Status == (int)DataStatus.Normal)
                                , o => o.Id
                                , i => i.SourceId
                                , (o, i) => new { A = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() });

            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.A.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new Yintai.Hangzhou.Contract.DTO.Response.IMSComboDetailResponse().FromEntity<IMSComboDetailResponse>(l.A, o =>
            {
                if (l.R == null)
                    return;
                o.ImageUrl = l.R.Name;

            }));
            var response = new PagerInfoResponse<IMSComboDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSComboDetailResponse>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Products(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<ProductEntity>().Where(ia => ia.CreatedUser == authuid && ia.ProductType == (int)ProductType.FromSelf)
                       .GroupJoin(Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product && r.Status == (int)DataStatus.Normal),
                                    o => o.Id,
                                    i => i.SourceId,
                                    (o, i) => new { P = o, R = i.OrderByDescending(ir => ir.SortOrder).FirstOrDefault() });
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.P.CreatedDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new IMSProductSelfDetailResponse().FromEntity<IMSProductSelfDetailResponse>(l.P, o =>
            {

                if (l.R == null)
                    return;
                o.ImageUrl = l.R.Name;
            }));
            var response = new PagerInfoResponse<IMSProductSelfDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSProductSelfDetailResponse>>(c => c.Data = response);

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult SalesCodes(int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                            .GroupJoin(Context.Set<IMS_AssociateSaleCodeEntity>(),
                                        o => o.Id,
                                        i => i.AssociateId,
                                        (o, i) => i).FirstOrDefault();
            var salesCodes = linq == null ? new string[] { } : linq.Select(l => l.Code).ToArray();
            var response = new PagerInfoResponse<string>(new PagerRequest(), salesCodes.Count())
            {
                Items = salesCodes.ToList()
            };


            return this.RenderSuccess<PagerInfoResponse<string>>(c => c.Data = response);
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Avail_Banks()
        {
            var linq = Context.Set<IMS_BankEntity>().Where(ia => ia.Status == (int)DataStatus.Normal).OrderBy(l=>l.Name);
            var response = new PagerInfoResponse<dynamic>(new PagerRequest(), linq.Count())
            {
                Items = linq.ToList().Select(l=>new {
                    code = l.Code,
                    name = l.Name
                }).ToList<dynamic>()
            };


            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult SalesCode_Add(string sale_code, int authuid)
        {
            if (string.IsNullOrEmpty(sale_code))
                return this.RenderError(r => r.Message = "销售码为空");
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid).FirstOrDefault();
            if (linq == null)
                return this.RenderError(r => r.Message = "无权操作");

            _salescodeRepo.Insert(new IMS_AssociateSaleCodeEntity()
            {
                AssociateId = linq.Id,
                Code = sale_code,
                CreateDate = DateTime.Now,
                CreateUser = authuid,
                Status = (int)DataStatus.Normal,
                UserId = authuid
            });


            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Brands(int authuid)
        {
            var linq = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                            .GroupJoin(Context.Set<IMS_AssociateBrandEntity>().Join(Context.Set<BrandEntity>().Where(b => b.Status == (int)DataStatus.Normal),
                                                                    o => o.BrandId,
                                                                    i => i.Id,
                                                                    (o, i) => new { AB = o, B = i }),
                                        o => o.Id,
                                        i => i.AB.AssociateId,
                                        (o, i) => i).FirstOrDefault();
            List<dynamic> brands = null;
            if (linq != null)
                brands = linq.Select(l => new
                {
                    id = l.AB.BrandId,
                    name = l.B.Name
                }).ToList<dynamic>();
            else
                brands = new List<dynamic>();
            var response = new PagerInfoResponse<dynamic>(new PagerRequest(), brands.Count())
            {
                Items = brands
            };


            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Combo_Status_Update(IMSComboStatusUpdateRequest request, int authuid)
        {
            var comboItemEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid)
                                .Join(Context.Set<IMS_AssociateItemsEntity>().Where(iai => iai.ItemType == request.Item_Type && iai.ItemId == request.Item_Id), o => o.Id, i => i.AssociateId, (o, i) => i)
                                .FirstOrDefault();
            if (comboItemEntity == null)
                return this.RenderError(r => r.Message = "无权操作该搭配");
            using (var ts = new TransactionScope())
            {
                comboItemEntity.Status = request.Is_Online?(int)DataStatus.Normal:(int)DataStatus.Default;
                comboItemEntity.UpdateDate = DateTime.Now;
                _associateitemRepo.Update(comboItemEntity);

                if (request.Is_Online &&
                    request.Item_Type == (int)ComboType.Product)
                {
                    var inventories = Context.Set<ProductEntity>().Where(p => p.ProductType == (int)ProductType.FromSelf)
                                    .Join(Context.Set<IMS_Combo2ProductEntity>().Where(ic => ic.ComboId == request.Item_Id),
                                         o => o.Id,
                                         i => i.ProductId,
                                         (o, i) => o)
                                     .Join(Context.Set<InventoryEntity>(),
                                                o=>o.Id,
                                                i=>i.ProductId,
                                                (o,i)=>i);
                    foreach (var inventory in inventories)
                    {
                        if (inventory.Amount < 1)
                        {
                            inventory.Amount = 1;
                            inventory.UpdateDate = DateTime.Now;
                            _inventoryRepo.Update(inventory);
                        }
                    }
                }
                ts.Complete();
            }
            return this.RenderSuccess<dynamic>(null);

        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Income(int authuid)
        {
            var incomeEntity = Context.Set<IMS_AssociateIncomeEntity>().Where(iai => iai.UserId == authuid).FirstOrDefault();
            if (incomeEntity == null)
                incomeEntity = new IMS_AssociateIncomeEntity()
                {
                    ReceivedAmount = 0,
                    AvailableAmount = 0,
                    TotalAmount = 0,
                    RequestAmount = 0


                };
            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                received_amount = incomeEntity.ReceivedAmount,
                frozen_amount = incomeEntity.TotalAmount - incomeEntity.AvailableAmount,
                request_amount = incomeEntity.RequestAmount,
                avail_amount = incomeEntity.AvailableAmount

            });
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Income_Received(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<IMS_AssociateIncomeRequestEntity>().
                        Where(iair => iair.UserId == authuid && iair.Status == (int)AssociateIncomeRequestStatus.Transferred);
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList().Select(l => new IMSIncomeReqDetailResponse().FromEntity<IMSIncomeReqDetailResponse>(l));
            var response = new PagerInfoResponse<IMSIncomeReqDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSIncomeReqDetailResponse>>(c => c.Data = response);
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Income_Frozen(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<IMS_AssociateIncomeHistoryEntity>().
                        Where(iair => iair.AssociateUserId == authuid && iair.Status == (int)AssociateIncomeStatus.Frozen && iair.SourceType == (int)AssociateOrderType.GiftCard)
                        .Join(Context.Set<IMS_GiftCardOrderEntity>(), o => o.SourceNo, i => i.No, (o, i) => new IMSIncomeDetailResponse
                         { 
                            CreateDate = o.CreateDate,
                            Id = i.Id,
                             AssociateIncome = o.AssociateIncome,
                              SourceNo =o.SourceNo,
                               TotalAmount = i.Amount,
                                SourceType = o.SourceType,
                                 Status = o.Status
                        });

            var linq2 = Context.Set<IMS_AssociateIncomeHistoryEntity>().
                        Where(iair => iair.AssociateUserId == authuid && iair.Status == (int)AssociateIncomeStatus.Frozen && iair.SourceType == (int)AssociateOrderType.GiftCard)
                        .Join(Context.Set<OrderEntity>(), o => o.SourceNo, i => i.OrderNo, (o, i) => new IMSIncomeDetailResponse
                        {
                            CreateDate = o.CreateDate,
                            Id = i.Id,
                            AssociateIncome = o.AssociateIncome,
                            SourceNo = o.SourceNo,
                            TotalAmount = i.TotalAmount,
                            SourceType = o.SourceType,
                            Status = o.Status
                        });
            linq = linq.Union(linq2);
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var result = linq.ToList();
            var response = new PagerInfoResponse<IMSIncomeDetailResponse>(request.PagerRequest, totalCount)
            {
                Items = result.ToList()
            };

            return this.RenderSuccess<PagerInfoResponse<IMSIncomeDetailResponse>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Income_Request(IMSIncomeReqRequest request, int authuid)
        {
            //CHECK THE income request rule
            if (string.IsNullOrEmpty(request.Bank_No))
                return this.RenderError(r => r.Message = "银行卡号不能为空");
            if (string.IsNullOrEmpty(request.User_Name))
                return this.RenderError(r => r.Message = "银行帐户名不能为空");

            var incomeAccountEntity = Context.Set<IMS_AssociateIncomeEntity>().Where(iai => iai.UserId == authuid).FirstOrDefault();
            if (incomeAccountEntity == null)
                return this.RenderError(r => r.Message = "账户内可提现金额不足！");
            if (incomeAccountEntity.AvailableAmount < request.Amount)
                return this.RenderError(r => r.Message = "账户内可体现金额不足!");
            var incomeRequestHistory = Context.Set<IMS_AssociateIncomeRequestEntity>().Where(iair => iair.UserId == authuid && iair.Status != (int)AssociateIncomeRequestStatus.Failed);
            var availLimitAmount = ConfigManager.IMS_MAX_REQUEST_AMOUNT_MON - incomeRequestHistory.Sum(l => l.Amount);
            if (request.Amount > availLimitAmount)
                return this.RenderError(r => r.Message = string.Format("每月累计提现额度为:{0},本月还可提现:{1}",
                                ConfigManager.IMS_MAX_REQUEST_AMOUNT_MON,
                                availLimitAmount));
            var bankEntity = Context.Set<IMS_BankEntity>().
                        Where(ib => ib.Status == (int)DataStatus.Normal && ib.Code == request.Bank_Code).
                        FirstOrDefault();
            if (bankEntity == null)
                return this.RenderError(r => r.Message = "银行不支持提现");
            _incomerequestRepo.Insert(new IMS_AssociateIncomeRequestEntity()
            {
                Amount = request.Amount,
                BankName = bankEntity.Name,
                CreateDate = DateTime.Now,
                BankNo = request.Bank_No,
                BankCode = request.Bank_Code,
                Status = (int)AssociateIncomeRequestStatus.Requesting,
                UpdateDate = DateTime.Now,
                UserId = authuid


            });

            return this.RenderSuccess<dynamic>(null);
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Order_GiftCards(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<IMS_AssociateIncomeHistoryEntity>().
                        Where(iai => iai.AssociateUserId == authuid && iai.SourceType == (int)AssociateOrderType.Product)
                        .Join(Context.Set<IMS_GiftCardOrderEntity>(), o => o.SourceNo, i => i.No, (o, i) => new
                        {
                            A = o,
                            O = i
                        });
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.A.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, totalCount)
            {
                Items = linq.ToList().Select(l => new
                {
                    amount = l.O.Amount,
                    create_date = l.O.CreateDate,
                }).ToList<dynamic>()
            };

            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Orders(PagerInfoRequest request, int authuid)
        {
            var linq = Context.Set<IMS_AssociateIncomeHistoryEntity>().
                        Where(iai => iai.AssociateUserId == authuid && iai.SourceType == (int)AssociateOrderType.Product)
                        .Join(Context.Set<OrderEntity>(), o => o.SourceNo, i => i.OrderNo, (o, i) => new
                        {
                            A = o,
                            O = i
                        });
            int totalCount = linq.Count();
            int skipCount = request.Page > 0 ? (request.Page - 1) * request.Pagesize : 0;
            linq = linq.OrderByDescending(l => l.A.CreateDate).Skip(skipCount).Take(request.Pagesize);
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, totalCount)
            {
                Items = linq.ToList().Select(l => new
                {
                    order_no = l.A.SourceNo,
                    amount = l.O.TotalAmount,
                    create_date = l.O.CreateDate,
                    status = ((OrderStatus)l.O.Status).ToFriendlyString()
                }).ToList<dynamic>()
            };

            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);

        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update_Name(string name, int authuid)
        {
            if (string.IsNullOrEmpty(name))
                return this.RenderError(r => r.Message = "店铺名称不能为空");
            var userEntity = Context.Set<UserEntity>().Find(authuid);
            userEntity.Nickname = name;
            userEntity.UpdatedDate = DateTime.Now;
            userEntity.UpdatedUser = authuid;

            _userRepo.Update(userEntity);
            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                name = userEntity.Nickname
            });
        }
        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update_Mobile(string mobile, int authuid)
        {
            var userEntity = Context.Set<UserEntity>().Find(authuid);
            userEntity.Mobile = mobile;
            userEntity.UpdatedDate = DateTime.Now;
            userEntity.UpdatedUser = authuid;

            _userRepo.Update(userEntity);
            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                phone = userEntity.Mobile
            });
        }



        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Update_Template(int templateId, int authuid)
        {
            var associateEntity = Context.Set<IMS_AssociateEntity>().Where(ia => ia.UserId == authuid).FirstOrDefault();
            associateEntity.TemplateId = templateId;
            _associateRepo.Update(associateEntity);
            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulRoleAuthorize(UserLevel.DaoGou)]
        public ActionResult Feedback(string comment, int authuid)
        {
            if (string.IsNullOrEmpty(comment))
                return this.RenderError(r => r.Message = "意见不能为空");
            _feedbackRepo.Insert(new FeedbackEntity()
            {
                Content = comment,
                Contact = string.Empty,
                CreatedDate = DateTime.Now,
                CreatedUser = authuid,
                Status = (int)DataStatus.Normal,
                UpdatedDate = DateTime.Now,
                UpdatedUser = authuid,
                User_Id = authuid
            });
            return this.RenderSuccess<dynamic>(null);
        }

    }
}
