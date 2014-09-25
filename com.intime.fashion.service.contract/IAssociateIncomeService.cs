using System;
namespace com.intime.fashion.service.contract
{
   public interface IAssociateIncomeService
    {
        bool Avail(int associateUserId, Yintai.Hangzhou.Data.Models.IMS_GiftCardOrderEntity giftOrder);
        bool Avail(Yintai.Hangzhou.Data.Models.IMS_AssociateIncomeHistoryEntity incomeHistory);
        bool Create(Yintai.Hangzhou.Data.Models.OrderEntity order);
        bool Froze(string orderNo);
        bool Void(Yintai.Hangzhou.Data.Models.IMS_AssociateIncomeHistoryEntity incomeHistory);
    }
}
