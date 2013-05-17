using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class LikeEntity : IAccountable
    {


        public int AccountUserId
        {
            get { return this.LikeUserId; }
        }

        public void AccountSyncing(int userId)
        {
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var likedAccount = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.LikeMeCount && ua.User_Id == LikedUserId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var liked = db.Likes.Where(p => p.LikedUserId == userId && p.Status != (int)DataStatus.Deleted).Count();
                if (likedAccount!= null)
                {
                    likedAccount.Amount = liked;
                    likedAccount.UpdatedDate = DateTime.Now;
                    db.Entry(likedAccount).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.UserAccounts.Add(new UserAccountEntity()
                    {
                        AccountType = (int)AccountType.LikeMeCount,
                        Amount = liked,
                        User_Id = LikedUserId,
                        Status = (int)DataStatus.Normal,
                        CreatedDate = DateTime.Now,
                        CreatedUser = LikedUserId,
                        UpdatedDate = DateTime.Now,
                        UpdatedUser = LikedUserId
                    });
                }
                var account = db.UserAccounts.Where(ua => ua.AccountType == (int)AccountType.IlikeCount && ua.User_Id == userId && ua.Status != (int)DataStatus.Deleted).FirstOrDefault();
                var likes = db.Likes.Where(p => p.LikeUserId == userId && p.Status != (int)DataStatus.Deleted).Count();
                if (account != null)
                {
                    account.Amount = likes;
                    account.UpdatedDate = DateTime.Now;
                    db.Entry(account).State = System.Data.EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    db.UserAccounts.Add(new UserAccountEntity()
                    {
                        AccountType = (int)AccountType.IlikeCount,
                        Amount = likes,
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
