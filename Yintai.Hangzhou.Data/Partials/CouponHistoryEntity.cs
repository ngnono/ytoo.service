using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class CouponHistoryEntity: ISyncable,IAccountable
    {
        public string TypeName
        {
            get { return "promotioncoupon"; }
        }

        public int SyncId
        {
            get { return this.Id; }
        }
        public object Composing()
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var linq = db.CouponHistories.Find(this.SyncId);
                if (linq == null)
                    return null;
                return new
                {
                    id = linq.Id,
                    status = linq.Status,
                    code = linq.CouponId,
                    amount = 0m,
                    userid = linq.User_Id,
                    validstartdate = linq.ValidStartDate,
                    validenddate = linq.ValidEndDate,
                    vipcard = 0

                };
            }
        }

        public int AccountUserId
        {
            get { return this.User_Id; }
        }

        public void AccountSyncing(int userId)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var account = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.Coupon && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var sumCoupons = db.CouponHistories.Where(p => p.User_Id == userId && p.Status != (int)DataStatus.Deleted && p.ValidEndDate>=DateTime.Now && p.Status!=(int)CouponStatus.Used).Count();
                var sumStoreCoupons = db.StoreCoupons.Where(p => p.UserId == userId && p.Status != (int)DataStatus.Deleted && p.ValidEndDate >= DateTime.Now && p.Status != (int)CouponStatus.Used).Count();
                if (account != null)
                {
                    account.Amount = sumCoupons+sumStoreCoupons;
                    account.UpdatedDate = DateTime.Now;
                    db.Entry(account).State = System.Data.EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    db.UserAccounts.Add(new UserAccountEntity()
                    {
                        AccountType = (int)AccountType.Coupon,
                        Amount = sumCoupons + sumStoreCoupons,
                        User_Id = userId,
                        Status = (int)DataStatus.Normal,
                        CreatedDate = DateTime.Now,
                        CreatedUser = userId,
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = userId
                    });
                }
            }
        }
    }
}
