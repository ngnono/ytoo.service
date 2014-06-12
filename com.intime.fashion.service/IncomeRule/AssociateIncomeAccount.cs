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

namespace com.intime.fashion.service.IncomeRule
{
    public class AssociateIncomeAccount
    {
        public static bool Froze(int userId, decimal amount)
        {
            var account = EnsureAccount(userId);
            account.TotalAmount += amount;
            account.UpdateDate = DateTime.Now;
            AccountRepo.Update(account);
            return true;
        }
        public static bool Avail(int userId, decimal amount)
        {
            var account = EnsureAccount(userId);
            if ((account.TotalAmount - account.AvailableAmount) <
                amount)
                return false;
            account.AvailableAmount += amount;
            account.UpdateDate = DateTime.Now;
            AccountRepo.Update(account);
            return true;
        }
        public static bool AvailGift(int userId, decimal amount)
        {
            var account = EnsureAccount(userId);
            account.AvailableAmount += amount;
            account.TotalAmount += amount;
            account.UpdateDate = DateTime.Now;
            AccountRepo.Update(account);
            return true;
        }
        public static bool Void(int userId, decimal amount)
        {
            var account = EnsureAccount(userId);
            if ((account.TotalAmount - account.AvailableAmount) < amount)
                return false;
            account.TotalAmount -= amount;
            account.UpdateDate = DateTime.Now;
            AccountRepo.Update(account);
            return true;
        }
        private static IMS_AssociateIncomeEntity EnsureAccount(int userId)
        {
            var account = Context.Set<IMS_AssociateIncomeEntity>().Where(ia => ia.UserId == userId && ia.Status == (int)DataStatus.Normal).FirstOrDefault();
            if (account == null)
            {
                account = AccountRepo.Insert(new IMS_AssociateIncomeEntity()
                {
                    AvailableAmount = 0m,
                    CreateDate = DateTime.Now,
                    ReceivedAmount = 0m,
                    RequestAmount = 0m,
                    Status = (int)DataStatus.Normal,
                    TotalAmount = 0m,
                    UpdateDate = DateTime.Now,
                    UserId = userId
                });
            }
            return account;
        }
        private static bool MakeIncomeAvail2Account(int userId, decimal availAmount)
        {
            var incomeAccountRepo = AccountRepo;
            var account = Context.Set<IMS_AssociateIncomeEntity>().Where(ia => ia.UserId == userId && ia.Status == (int)DataStatus.Normal).FirstOrDefault();
            if (account == null)
            {
                incomeAccountRepo.Insert(new IMS_AssociateIncomeEntity()
                {
                    AvailableAmount = availAmount,
                    TotalAmount = availAmount,
                    CreateDate = DateTime.Now,
                    ReceivedAmount = 0m,
                    RequestAmount = 0m,
                    Status = (int)DataStatus.Normal,
                    UserId = userId,
                    UpdateDate = DateTime.Now
                });
            }
            else
            {
                account.AvailableAmount += availAmount;
                account.UpdateDate = DateTime.Now;
                incomeAccountRepo.Update(account);
            }
            return true;
        }
        private static IEFRepository<IMS_AssociateIncomeEntity> AccountRepo { get {
            return ServiceLocator.Current.Resolve<IEFRepository<IMS_AssociateIncomeEntity>>();
        } }
        private static DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }
    }
}
