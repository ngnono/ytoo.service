﻿using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Logic.IncomeRule;

namespace Yintai.Hangzhou.Service.Logic
{
    public static class AssociateIncomeLogic
    {
       
        public static bool Create(int associateUserId, OrderEntity order)
        {
            var incomeRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateIncomeHistoryEntity>>();
            incomeRepo.Insert(new IMS_AssociateIncomeHistoryEntity()
            {
                CreateDate = DateTime.Now,
                SourceNo = order.OrderNo,
                SourceType = (int)AssociateOrderType.Product,
                Status = (int)AssociateIncomeStatus.Create,
                UpdateDate = DateTime.Now,
                AssociateIncome = ComputeIncome(order),
                AssociateUserId = associateUserId
            });
            return true;
        }
        public static bool Froze(string orderNo)
        {
            var incomeRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateIncomeHistoryEntity>>();
            foreach (var item in Context.Set<IMS_AssociateIncomeHistoryEntity>().Where(iai => iai.SourceType == (int)AssociateOrderType.Product && iai.SourceNo == orderNo))
            {
                item.Status = (int)AssociateIncomeStatus.Frozen;
                item.UpdateDate = DateTime.Now;
                incomeRepo.Update(item);
            }
            return true;
        }
        public static bool Froze(int associateUserId, IMS_GiftCardOrderEntity giftOrder)
        {
            var incomeRepo = ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateIncomeHistoryEntity>>();
            incomeRepo.Insert(new IMS_AssociateIncomeHistoryEntity()
            {
                CreateDate = DateTime.Now,
                SourceNo = giftOrder.No,
                SourceType = (int)AssociateOrderType.GiftCard,
                Status = (int)AssociateIncomeStatus.Frozen,
                UpdateDate = DateTime.Now,
                AssociateIncome = ComputeIncome(giftOrder),
                AssociateUserId = associateUserId
            });
            return true;
        }
        public static decimal ComputeIncome(Data.Models.IMS_GiftCardOrderEntity giftcardOrder)
        {
            if (giftcardOrder == null)
                return 0m;
            return DoInternalCompute(giftcardOrder.Price, ConfigManager.IMS_GIFTCARD_CAT_ID, 1);
        }
        public static decimal ComputeIncome(Data.Models.OrderEntity order)
        {
            if (order == null)
                return 0m;
            var incomeSum = 0m;
            foreach (var item in Context.Set<OrderItemEntity>().Where(o => o.OrderNo == order.OrderNo && o.Status == (int)DataStatus.Normal))
            { 
                var product = Context.Set<ProductEntity>().Find(item.ProductId);
                incomeSum += DoInternalCompute(item.ItemPrice, product.Tag_Id, item.Quantity);
            }
            return incomeSum;
        }

        private static decimal DoInternalCompute(decimal price, int categoryId, int quantity)
        {
            var incomeRuleEntity = Context.Set<IMS_AssociateIncomeRuleEntity>().Where(iar => iar.Status == (int)DataStatus.Normal &&
                            iar.FromDate <= DateTime.Now && iar.EndDate > DateTime.Now && iar.CategoryId == categoryId).FirstOrDefault();
            if (incomeRuleEntity == null)
                return 0m;
            
            IIncomeRule rule = CreateRule(incomeRuleEntity.RuleType);
            if (rule == null)
                return 0m;
            return rule.Compute(incomeRuleEntity.Id, price, quantity);
        }
        private static IIncomeRule CreateRule(int ruleType)
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
        private static DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }

    }
}
