using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using com.intime.fashion.data.sync;
using Common.Logging;
using Quartz;
using Quartz.Simpl;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.jobscheduler.Job.Store
{
    public class StoreApplyApproveJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ILog logger = LogManager.GetCurrentClassLogger();
            IApplyInfoStore store = new DefaultApplyInfoStore();
            IApplyInfoProcessor processor = new DefaultApplyInfoProcessor();

        }
    }

    public class StoreApplyExecutor
    {
        private ILog _logger;
        private IApplyInfoProcessor _processor;
        private IApplyInfoStore _store;
        public StoreApplyExecutor(ILog logger, IApplyInfoStore store, IApplyInfoProcessor processor)
        {
            this._logger = logger;
            this._store = store;
            this._processor = processor;
        }

        public void Execute()
        {
            int totalCount = 0;
            int cursor = 0;
            int lastCursor = 0;
            int pageSize = 20;
            while (cursor < totalCount)
            {
                var oneTimeList = _store.FetchRequest(cursor, pageSize, lastCursor);
                foreach (var entity in oneTimeList)
                {
                    try
                    {
                        _processor.Process(entity);
                    }
                    catch (ApplyInfoProcessException ex)
                    {
                        _logger.Error(string.Format("Process invite code error, invitecode request id{0}, error message{1}", entity.Id, ex.Message));
                    }
                }
                cursor += pageSize;
                lastCursor += oneTimeList.Max(x => x.Id);
            }
        }
    }

    public class DefaultApplyInfoProcessor : IApplyInfoProcessor
    {
        public void Process(IMS_InviteCodeRequestEntity storeRequest)
        {
            using (var Context = DbContextHelper.GetDbContext())
            {
                var exitUserEntity = Context.Set<UserEntity>().Find(storeRequest.UserId);

                //steps:
                //// 1. read initial info from invite code tables
                //// 2. initialize associate tables
                //using (var ts = new TransactionScope())
                //{
                //    var initialBrands = Context.Set<IMS_SectionBrandEntity>().Where(isb => isb.SectionId == inviteEntity.Sec.SectionId);
                //    var initialSaleCodes = Context.Set<IMS_SalesCodeEntity>().Where(isc => isc.SectionId == inviteEntity.Sec.SectionId);
                //    var sectionEntity = Context.Set<SectionEntity>().Find(inviteEntity.Sec.SectionId);
                //    //2.0 disable invite code
                //    inviteEntity.Inv.IsBinded = 1;
                //    inviteEntity.Inv.UpdateDate = DateTime.Now;
                //    inviteEntity.Inv.UserId = authuid.Value;
                //    _inviteRepo.Update(inviteEntity.Inv);

                //    //2.1 update user level to daogou
                //    exitUserEntity.UserLevel = (int)UserLevel.DaoGou;
                //    exitUserEntity.UpdatedDate = DateTime.Now;
                //    exitUserEntity.UpdatedUser = exitUserEntity.Id;
                //    if (string.IsNullOrEmpty(exitUserEntity.Logo))
                //        exitUserEntity.Logo = ConfigManager.IMS_DEFAULT_LOGO;
                //    _customerRepo.Update(exitUserEntity);

                //    //2.2 create daogou's associate store
                //    var assocateEntity = _associateRepo.Insert(new IMS_AssociateEntity()
                //    {
                //        CreateDate = DateTime.Now,
                //        CreateUser = authuid.Value,
                //        OperateRight = inviteEntity.Inv.AuthRight.Value,
                //        Status = (int)DataStatus.Normal,
                //        TemplateId = ConfigManager.IMS_DEFAULT_TEMPLATE,
                //        UserId = authuid.Value,
                //        StoreId = sectionEntity.StoreId ?? 0,
                //        SectionId = sectionEntity.Id,
                //        OperatorCode = inviteEntity.Sec.OperatorCode
                //    });
                //    //2.3 create daogou's brands
                //    foreach (var brand in initialBrands)
                //    {
                //        _associateBrandRepo.Insert(new IMS_AssociateBrandEntity()
                //        {
                //            AssociateId = assocateEntity.Id,
                //            BrandId = brand.BrandId,
                //            CreateDate = DateTime.Now,
                //            CreateUser = authuid.Value,
                //            Status = (int)DataStatus.Normal,
                //            UserId = authuid.Value
                //        });
                //    }
                //    //2.4 create daogou's sales code
                //    foreach (var saleCode in initialSaleCodes)
                //    {
                //        _associateSaleCodeRepo.Insert(new IMS_AssociateSaleCodeEntity()
                //        {
                //            AssociateId = assocateEntity.Id,
                //            Code = saleCode.Code,
                //            CreateDate = DateTime.Now,
                //            CreateUser = authuid.Value,
                //            Status = (int)DataStatus.Normal,
                //            UserId = authuid.Value

                //        });
                //    }
                //    //2.5 create daogou's giftcard
                //    var giftCardEntity = Context.Set<IMS_GiftCardEntity>().Where(igc => igc.Status == (int)DataStatus.Normal).FirstOrDefault();
                //    if (giftCardEntity != null)
                //    {
                //        _associateItemRepo.Insert(new IMS_AssociateItemsEntity()
                //        {
                //            AssociateId = assocateEntity.Id,
                //            CreateDate = DateTime.Now,
                //            CreateUser = authuid.Value,
                //            ItemId = giftCardEntity.Id,
                //            ItemType = (int)ComboType.GiftCard,
                //            Status = (int)DataStatus.Normal,
                //            UpdateDate = DateTime.Now,
                //            UpdateUser = authuid.Value
                //        });
                //    }
                //    ts.Complete();

                //}
            }
        }
    }

    public interface IApplyInfoProcessor
    {
        void Process(IMS_InviteCodeRequestEntity storeRequest);
    }

    public class ApplyInfoProcessException : Exception
    {

        public ApplyInfoProcessException(string message)
            : base(message)
        {

        }
    }

    public interface IApplyInfoStore
    {
        IEnumerable<IMS_InviteCodeRequestEntity> FetchRequest(int pageIndex, int pageSize, int benchmarkId);

        int QueryCount();
    }

    public class DefaultApplyInfoStore : IApplyInfoStore
    {
        private void DoQuery(Expression<Func<IMS_InviteCodeRequestEntity, bool>> whereCondition, Action<IQueryable<IMS_InviteCodeRequestEntity>> callback)
        {
            using (var context = DbContextHelper.GetDbContext())
            {
                var linq = context.IMS_InviteCodeRequest.Where(x => !x.Approved.HasValue && ! x.Approved.HasValue && x.Status == (int)InviteCodeRequestStatus.Requesting && x.RequestType == (int)InviteCodeRequestType.Daogou);

                if (whereCondition != null)
                    linq = linq.Where(whereCondition);
                if (callback != null)
                    callback(linq);
            }
        }

        public IEnumerable<IMS_InviteCodeRequestEntity> FetchRequest(int pageIndex, int pageSize, int benchmarkId)
        {
            List<IMS_InviteCodeRequestEntity> list = null;
            DoQuery(null, x => list = x.Where(o => o.Id > benchmarkId).OrderBy(o => o.Id).Take(pageSize).ToList());
            return list;
        }

        public int QueryCount()
        {
            int total = 0;
            DoQuery(null, x => total = x.Count());
            return total;
        }
    }
}
