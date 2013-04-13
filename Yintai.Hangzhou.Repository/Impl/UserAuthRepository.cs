using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public class UserAuthRepository : RepositoryBase<UserAuthEntity, int>, IUserAuthRepository
    {
        public IQueryable<dynamic> AuthFilter(IQueryable<dynamic> query, int userId, UserRole userRole)
        {
            if (userRole == UserRole.Admin)
                return query;
            if (query is IQueryable<ProductEntity>)
            {
                return from q in (query as IQueryable<ProductEntity>)
                       where Context.Set<UserAuthEntity>().Where(a => a.UserId == userId && a.Type == (int)AuthDataType.Product)
                            .Any(a =>a.StoreId == q.Store_Id &&
                                        (a.BrandId == 0 || a.BrandId == q.Brand_Id))
                       select q;
               
                
            } else if (query is IQueryable<PromotionEntity>)
            {
                return from q in (query as IQueryable<PromotionEntity>)
                       where Context.Set<UserAuthEntity>().Where(a => a.UserId == userId && a.Type == (int)AuthDataType.Promotion)
                            .Any(a => a.StoreId == q.Store_Id)
                       select q;
            }
          
            return query;
        }

       
      
    }
}
