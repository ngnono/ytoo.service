using System;
using System.Linq;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class PointHistoryEntity : IAccountable
    {
        public int AccountUserId
        {
            get { return this.User_Id; }
        }

        public void AccountSyncing(int userId)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
               var account = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.Point && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
               var sumPoints = db.PointHistories.Where(p => p.User_Id == userId && p.Status != (int)DataStatus.Deleted).Sum(p => p.Amount);
               if (account != null)
               {
                   account.Amount = sumPoints > 0 ? sumPoints : 0;
                   account.UpdatedDate = DateTime.Now;
                   db.Entry(account).State = System.Data.EntityState.Modified;
                   db.SaveChanges();

               }
               else
               {
                   db.UserAccounts.Add(new UserAccountEntity()
                   {
                       AccountType = (int)AccountType.Point,
                       Amount = sumPoints > 0 ? sumPoints : 0,
                       User_Id = userId,
                       Status = (int)DataStatus.Normal,
                       CreatedDate = DateTime.Now,
                       CreatedUser = userId,
                       UpdatedDate = DateTime.Now,
                       UpdatedUser = userId
                   });
                   db.SaveChanges();
               }
            }
        }
    }
}