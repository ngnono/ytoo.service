using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class StoreCouponEntity : ISyncable,IAccountable
    {
        public string TypeName
        {
            get { return "storecoupon"; }
        }

        public int SyncId
        {
            get { return this.Id; }
        }
        public object Composing()
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var linq = db.StoreCoupons.Find(this.SyncId);
                if (linq == null)
                    return null;
                return new
                {
                    id = linq.Id,
                    status = linq.Status,
                    code = linq.Code,
                    amount = linq.Amount,
                    userid = linq.UserId,
                    validstartdate = linq.ValidStartDate.GetValueOrDefault().ToUniversalTime(),
                    validenddate = linq.ValidEndDate.GetValueOrDefault().ToUniversalTime(),
                    vipcard = linq.VipCard,
                    lastupdate = linq.UpdateDate.GetValueOrDefault().ToUniversalTime()

                };
            }
        }

        int IAccountable.AccountUserId
        {
            get { return this.UserId.Value; }
        }

        public void AccountSyncing(int userId)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var offlineAccount = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.OfflineCoupon && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var account = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.Coupon && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var sumCoupons = db.CouponHistories.Where(p => p.User_Id == userId && p.Status != (int)DataStatus.Deleted && p.ValidEndDate >= DateTime.Now && p.Status != (int)CouponStatus.Used).Count();
                var sumStoreCoupons = db.StoreCoupons.Where(p => p.UserId == userId && p.Status != (int)DataStatus.Deleted && p.ValidEndDate >= DateTime.Now && p.Status != (int)CouponStatus.Used).Count();
                if (offlineAccount != null)
                {
                    offlineAccount.Amount = sumStoreCoupons;
                    offlineAccount.UpdatedDate = DateTime.Now;
                    db.Entry(offlineAccount).State = System.Data.EntityState.Modified;
                }
                else
                {
                    db.UserAccounts.Add(new UserAccountEntity()
                    {
                        AccountType = (int)AccountType.OfflineCoupon,
                        Amount = sumStoreCoupons,
                        User_Id = userId,
                        Status = (int)DataStatus.Normal,
                        CreatedDate = DateTime.Now,
                        CreatedUser = userId,
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = userId
                    });
                }
                if (account != null)
                {
                    account.Amount = sumCoupons + sumStoreCoupons;
                    account.UpdatedDate = DateTime.Now;
                    db.Entry(account).State = System.Data.EntityState.Modified;


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
                db.SaveChanges();
            }
        }
    }
}
