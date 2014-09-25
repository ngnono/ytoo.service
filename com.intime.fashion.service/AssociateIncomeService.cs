using com.intime.fashion.common;
using com.intime.fashion.service.contract;
using com.intime.fashion.service.IncomeRule;
using com.intime.fashion.service.PromotionRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace com.intime.fashion.service
{
    public class AssociateIncomeService : BusinessServiceBase, IAssociateIncomeService
    {
        private IEFRepository<IMS_AssociateIncomeHistoryEntity> _incomeHistoryRepo;
        private IEFRepository<IMS_AssociateIncomeEntity> _incomeAccountRepo;
        public AssociateIncomeService(IEFRepository<IMS_AssociateIncomeHistoryEntity> incomeHistoryRepo
            , IEFRepository<IMS_AssociateIncomeEntity> incomeAccountRepo
            )
            : base()
        {
            _incomeHistoryRepo = incomeHistoryRepo;
            _incomeAccountRepo = incomeAccountRepo;
        }
        public bool Create(OrderEntity order)
        {
            foreach (var orderItem in _db.Set<OrderItemEntity>().Where(oi => oi.OrderNo == order.OrderNo
                                        && oi.ProductType == (int)ProductType.FromSelf
                                        && oi.Status == (int)DataStatus.Normal))
            {
                var _associateUserEntity = _db.Set<ProductEntity>().Find(orderItem.ProductId);
                _incomeHistoryRepo.Insert(new IMS_AssociateIncomeHistoryEntity()
                {
                    CreateDate = DateTime.Now,
                    SourceNo = order.OrderNo,
                    SourceType = (int)AssociateOrderType.Product,
                    Status = (int)AssociateIncomeStatus.Create,
                    UpdateDate = DateTime.Now,
                    AssociateIncome = ComputeIncome(order, orderItem),
                    AssociateUserId = _associateUserEntity.CreatedUser
                });
            }

            return true;
        }
        public bool Froze(string orderNo)
        {
            orderNo = orderNo.Trim();

            foreach (var item in _db.Set<IMS_AssociateIncomeHistoryEntity>()
                        .Where(iai => iai.SourceType == (int)AssociateOrderType.Product && iai.SourceNo == orderNo)
                        .ToList())
            {
                item.Status = (int)AssociateIncomeStatus.Frozen;
                item.UpdateDate = DateTime.Now;
                _incomeHistoryRepo.Update(item);

                AssociateIncomeAccount.Froze(item.AssociateUserId, item.AssociateIncome);
            }
            return true;
        }
        public bool Avail(int associateUserId, IMS_GiftCardOrderEntity giftOrder)
        {

            var thisIncome = ComputeIncome(giftOrder);
            _incomeHistoryRepo.Insert(new IMS_AssociateIncomeHistoryEntity()
            {
                CreateDate = DateTime.Now,
                SourceNo = giftOrder.No,
                SourceType = (int)AssociateOrderType.GiftCard,
                Status = (int)AssociateIncomeStatus.Avail,
                UpdateDate = DateTime.Now,
                AssociateIncome = thisIncome,
                AssociateUserId = associateUserId
            });

            AssociateIncomeAccount.AvailGift(associateUserId, thisIncome);

            return true;
        }
        public bool Avail(IMS_AssociateIncomeHistoryEntity incomeHistory)
        {

            incomeHistory.Status = (int)AssociateIncomeStatus.Avail;
            incomeHistory.UpdateDate = DateTime.Now;
            _incomeHistoryRepo.Update(incomeHistory);
            AssociateIncomeAccount.Avail(incomeHistory.AssociateUserId, incomeHistory.AssociateIncome);
            return true;
        }
        public bool Void(IMS_AssociateIncomeHistoryEntity incomeHistory)
        {

            incomeHistory.Status = (int)AssociateIncomeStatus.Void;
            incomeHistory.UpdateDate = DateTime.Now;
            _incomeHistoryRepo.Update(incomeHistory);
            AssociateIncomeAccount.Void(incomeHistory.AssociateUserId, incomeHistory.AssociateIncome);
            return true;
        }


        private decimal ComputeIncome(IMS_GiftCardOrderEntity giftcardOrder)
        {
            if (giftcardOrder == null)
                return 0m;
            var group = _db.Set<IMS_GiftCardItemEntity>().Where(igi => igi.GiftCardId == giftcardOrder.GiftCardItemId)
                         .Join(_db.Set<IMS_GiftCardEntity>(), o => o.GiftCardId, i => i.Id, (o, i) => i)
                         .FirstOrDefault();
            return DoInternalCompute(giftcardOrder.Price.Value, ConfigManager.IMS_GIFTCARD_CAT_ID, 1, group == null ? null : group.GroupId);
        }
        private decimal ComputeIncome(OrderEntity order, OrderItemEntity orderItem)
        {
            if (order == null)
                return 0m;
            var incomeSum = 0m;
            var hasPromotion = order.PromotionFlag ?? false;
            IPromotionSharePolicy promotionSharedPolicy = null;
            if (hasPromotion)
            {
                promotionSharedPolicy = new PromotionService().GetDefaultSharePolicy();
                promotionSharedPolicy.SourceOrder = order;
            }

            var product = _db.Set<ProductEntity>().Find(orderItem.ProductId);
            var incomePrice = orderItem.ItemPrice;
            if (hasPromotion)
                incomePrice = promotionSharedPolicy.ComputeActualPrice(orderItem);
            var groupId = _db.Set<StoreEntity>().Find(orderItem.StoreId).Group_Id;

            incomeSum = DoInternalCompute(incomePrice, product.Tag_Id, orderItem.Quantity, groupId);
            return incomeSum;
        }
        private decimal DoInternalCompute(decimal price, int categoryId, int quantity, int? groupId = null)
        {
            var incomeRuleEntity = _db.Set<IMS_AssociateIncomeRuleEntity>().Where(iar => iar.Status == (int)DataStatus.Normal &&
                            iar.FromDate <= DateTime.Now && iar.EndDate > DateTime.Now
                            && iar.CategoryId == categoryId
                            && (groupId == null || iar.GroupId == groupId)).FirstOrDefault();
            if (incomeRuleEntity == null)
                return 0m;

            IIncomeRule rule = CreateRule(incomeRuleEntity.RuleType);
            if (rule == null)
                return 0m;
            return rule.Compute(incomeRuleEntity.Id, price, quantity);
        }
        private IIncomeRule CreateRule(int ruleType)
        {
            switch (ruleType)
            {
                case (int)IncomeRuleType.Fix:
                    return new IncomeRuleFix();
                case (int)IncomeRuleType.Flat:
                    return new IncomeRuleFlatten();
                case (int)IncomeRuleType.Flex:
                    return new IncomeRuleFlex();
                default:
                    return null;
            }
        }
    }
}
