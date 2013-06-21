using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class FavoriteEntity :  IAccountable
    {
      

        public int AccountUserId
        {
            get { return this.User_Id; }
        }

        public void AccountSyncing(int userId)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var account = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.FavorCount && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var favorites = db.Favorites.Where(p => p.User_Id == userId && p.Status != (int)DataStatus.Deleted ).Count();            
                if (account != null)
                {
                    account.Amount = favorites;
                    account.UpdatedDate = DateTime.Now;
                    db.Entry(account).State = System.Data.EntityState.Modified;
                    

                }
                else
                {
                    db.UserAccounts.Add(new UserAccountEntity()
                    {
                        AccountType = (int)AccountType.FavorCount,
                        Amount = favorites,
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
