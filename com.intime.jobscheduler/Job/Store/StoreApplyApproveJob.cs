﻿using com.intime.fashion.common;
using com.intime.fashion.common.message;
using com.intime.fashion.common.message.Messages;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job.Store
{
    public class StoreApplyApproveJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        public void Execute(IJobExecutionContext context)
        {
            int total = 0;
            int succeedCount = 0;
            DoQuery(requests => total = requests.Count());

            int cursor = 0;

            JobDataMap data = context.JobDetail.JobDataMap;
            int size = data.ContainsKey("pageSize") ? data.GetInt("pageSize") : 10;

            while (true)
            {
                List<IMS_InviteCodeRequestEntity> oneTimeList = null;
                DoQuery(requests => oneTimeList = requests.OrderBy(x => x.Id).Skip(cursor).Take(size).ToList());
                if (!oneTimeList.Any())
                {
                    break;
                }
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                var result = oneTimeList.AsParallel().Select(x => Task.Factory.StartNew(() => Initialize(x))).ToArray();
                Task.WaitAll(result);
                succeedCount += result.Count(x => x.Result);
                cursor += size;
            }
        }

        private bool Initialize(IMS_InviteCodeRequestEntity request)
        {
            using (var ts = new TransactionScope())
            using (var db = ServiceLocator.Current.Resolve<DbContext>())
            {
                var user = db.Set<UserEntity>().FirstOrDefault(x => x.Id == request.UserId);
                if (user == null)
                {
                    Logger.ErrorFormat("Invalid request {0}, user not exists", request.Id);
                    return false;
                }

                var requestEntity = db.Set<IMS_InviteCodeRequestEntity>().FirstOrDefault(x => x.Id == request.Id);
                if (requestEntity == null)
                {
                    Logger.ErrorFormat("Invalid request! id {0}", request.Id);
                    return false;
                }

                if (requestEntity.Approved.HasValue)
                {
                    Logger.ErrorFormat("Entity has been approved! {0}", request.Id);
                    return false;
                }

                requestEntity.Approved = true;
                requestEntity.ApprovedDate = DateTime.Now;
                requestEntity.ApprovedBy = request.UserId;
                requestEntity.UpdateDate = DateTime.Now;
                requestEntity.UpdateUser = request.UserId;
                requestEntity.Status = (int)InviteCodeRequestStatus.Approved;
                db.Entry(requestEntity).State = EntityState.Modified;

                if (user.UserLevel != (int)UserLevel.DaoGou)
                {
                    user.UserLevel = (int)UserLevel.DaoGou;
                    user.UpdatedDate = DateTime.Now;
                    user.UpdatedUser = request.UserId;
                    user.Logo = ConfigManager.IMS_DEFAULT_LOGO;
                    user.Mobile = request.ContactMobile;
                    db.Entry(user).State = EntityState.Modified;
                }

                var associate = db.Set<IMS_AssociateEntity>().FirstOrDefault(x => x.UserId == request.UserId);
                if (associate == null)
                {
                    associate = CreateAssociate(request);
                }
                else
                {
                    associate.OperateRight = request.RequestType == (int)InviteCodeRequestType.Daogou ? (int)(UserOperatorRight.GiftCard | UserOperatorRight.SelfProduct | UserOperatorRight.SystemProduct) : (int)(UserOperatorRight.GiftCard | UserOperatorRight.SystemProduct);
                    db.Entry(associate).State = EntityState.Modified;
                }

                var initialBrands = db.Set<IMS_SectionBrandEntity>().Where(x => x.SectionId == associate.SectionId);
                foreach (var brand in initialBrands)
                {
                    if (db.Set<IMS_AssociateBrandEntity>().Any(x => x.AssociateId == associate.Id && x.BrandId == brand.BrandId && x.Status == 1))
                        continue;
                    db.Set<IMS_AssociateBrandEntity>().Add(new IMS_AssociateBrandEntity
                    {
                        AssociateId = associate.Id,
                        BrandId = brand.BrandId,
                        CreateDate = DateTime.Now,
                        CreateUser = request.UserId,
                        Status = (int)DataStatus.Normal,
                        UserId = request.UserId
                    });
                }

                var initialSaleCodes = db.Set<IMS_SalesCodeEntity>().Where(x => x.SectionId == associate.SectionId);
                foreach (var saleCode in initialSaleCodes)
                {
                    if (db.Set<IMS_AssociateSaleCodeEntity>().Any(x => x.AssociateId == associate.Id && x.Code == saleCode.Code && x.Status == 1))
                        continue;
                    db.Set<IMS_AssociateSaleCodeEntity>().Add(new IMS_AssociateSaleCodeEntity
                    {
                        AssociateId = associate.Id,
                        Code = saleCode.Code,
                        CreateDate = DateTime.Now,
                        CreateUser = request.UserId,
                        Status = (int)DataStatus.Normal,
                        UserId = request.UserId

                    });
                }
                if (!db.Set<IMS_AssociateItemsEntity>().Any(x => x.AssociateId == associate.Id && x.Status == (int)DataStatus.Normal))
                {
                    var giftCards = db.Set<IMS_GiftCardEntity>().FirstOrDefault(x => x.Status == (int)DataStatus.Normal);
                    if (giftCards != null)
                    {
                        db.Set<IMS_AssociateItemsEntity>().Add(new IMS_AssociateItemsEntity
                        {
                            AssociateId = associate.Id,
                            CreateDate = DateTime.Now,
                            CreateUser = request.UserId,
                            ItemId = giftCards.Id,
                            ItemType = (int)ComboType.GiftCard,
                            Status = (int)DataStatus.Normal,
                            UpdateDate = DateTime.Now,
                            UpdateUser = request.UserId
                        });
                    }
                }

                db.SaveChanges();
                ts.Complete();
            }

            ServiceLocator.Current.Resolve<IMessageCenterProvider>()
                .GetSender()
                .SendMessageReliable(new ApprovedMessage()
                {
                    EntityId = request.Id
                });
            return true;
        }

        private IMS_AssociateEntity CreateAssociate(IMS_InviteCodeRequestEntity request)
        {
            using (var db = ServiceLocator.Current.Resolve<DbContext>())
            {
                var section =
                    db.Set<SectionEntity>()
                        .FirstOrDefault(x => x.SectionCode == request.SectionCode && x.StoreId == request.StoreId);
                if (section == null)
                {
                    return null;
                }

                var associate = new IMS_AssociateEntity()
                {
                    CreateDate = DateTime.Now,
                    CreateUser = request.UserId,
                    OperateRight = request.RequestType == (int)InviteCodeRequestType.Daogou ? (int)(UserOperatorRight.GiftCard | UserOperatorRight.SelfProduct | UserOperatorRight.SystemProduct) : (int)(UserOperatorRight.GiftCard | UserOperatorRight.SystemProduct),
                    SectionId = section.Id,
                    Status = (int)DataStatus.Normal,
                    StoreId = request.StoreId,
                    UserId = request.UserId,
                    TemplateId = ConfigManager.IMS_DEFAULT_TEMPLATE,
                    OperatorCode = request.OperatorCode,

                };

                associate = db.Set<IMS_AssociateEntity>().Add(associate);
                db.SaveChanges();
                return associate;
            }
        }


        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error(e);
            e.SetObserved();
        }


        private void DoQuery(Action<IQueryable<IMS_InviteCodeRequestEntity>> callback)
        {
            using (var context = ServiceLocator.Current.Resolve<DbContext>())
            {
                var linq = context.Set<IMS_InviteCodeRequestEntity>().Where(x => !x.Approved.HasValue);
                if (callback != null)
                {
                    callback(linq);
                }
            }
        }
    }
}
