using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class UserAccountRepository : RepositoryBase<UserAccountEntity, int>, IUserAccountRepository
    {
        #region Overrides of RepositoryBase<UserAccountEntity,int>

        /// <summary>
        /// 查找key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override UserAccountEntity GetItem(int key)
        {
            return base.Find(key);
        }

        public UserAccountEntity GetItem(int userId, AccountType accountType)
        {
            return base.Get(v => v.User_Id == userId && v.AccountType == (int)accountType).SingleOrDefault();
        }

        /// <summary>
        /// 获取当前用户的账户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserAccountEntity> GetUserAccount(int userId)
        {
            return base.Get(v => v.User_Id == userId).ToList();
        }

        public List<UserAccountEntity> GetListByUserIds(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return new List<UserAccountEntity>(0);
            }

            return base.Get(v => userIds.Any(s => s == v.User_Id)).ToList();
        }

        private int Ec(string amountString, int userId, AccountType accountType, decimal amount, int? updateUser)
        {
            var parames = new List<SqlParameter>
                {
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@User_Id", userId),
                    new SqlParameter("@AccountType", (int) accountType)
                };

            var update = String.Empty;

            if (updateUser != null)
            {
                parames.Add(new SqlParameter("@UpdatedUser", updateUser));
                parames.Add(new SqlParameter("@UpdatedDate", DateTime.Now));

                update = " ,[UpdatedUser] = @UpdatedUser,[UpdatedDate] = @UpdatedDate";
            }

            var sql =
    "UPDATE [dbo].[UserAccount] SET " + amountString + update + " WHERE [User_Id] = @User_Id AND [AccountType] = @AccountType;";
            /*
            var sql =
                "UPDATE [dbo].[UserAccount] SET    [Amount] = [Amount] + 1,[UpdatedUser] = 1,[UpdatedDate] = GETDATE() WHERE  [User_Id] = 1 AND [AccountType] = 7;";
            //*/
            //return base.ExecuteSqlCommand(sql, param);

            return SqlHelper.ExecuteNonQuery(SqlHelper.GetConnection(), CommandType.Text, sql, parames.ToArray());
        }

        public int AddCount(int userId, AccountType accountType, decimal amount, int? updateUser)
        {
            var entity = GetItem(userId, accountType);
            if (entity == null)
            {
                entity = this.Insert(new UserAccountEntity
                    {
                        AccountType = (int)accountType,
                        AccountId = 0,
                        Amount = amount,
                        CreatedDate = DateTime.Now,
                        CreatedUser = userId,
                        Status = (int)DataStatus.Normal,
                        UpdatedUser = updateUser ?? userId,
                        UpdatedDate = DateTime.Now,
                        User_Id = userId
                    });
                return 1;
            }

            const string t = " [Amount] = [Amount] + @Amount";

            return Ec(t, userId, accountType, amount, updateUser);
        }

        public int SubCount(int userId, AccountType accountType, decimal amount, int? updateUser)
        {
            var entity = GetItem(userId, accountType);
            if (entity == null)
            {
                entity = this.Insert(new UserAccountEntity
                {
                    AccountType = (int)accountType,
                    AccountId = 0,
                    Amount = 0,
                    CreatedDate = DateTime.Now,
                    CreatedUser = userId,
                    Status = (int)DataStatus.Normal,
                    UpdatedUser = updateUser ?? userId,
                    UpdatedDate = DateTime.Now,
                    User_Id = userId
                });

                return 0;
            }

            const string t = " [Amount] = [Amount] - @Amount";

            return Ec(t, userId, accountType, amount, updateUser);
        }

        #endregion
    }
}