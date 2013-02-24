using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Framework.Mapping;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class ImplicitRightViewModel
    {
        public string[] RoleRightDisplay { get; set; }
    }
    public class RoleViewModel:BaseViewModel
    {
        [Display(Name="角色编码")]
        public int Id { get; set; }
        [Display(Name = "角色名")]
        public string Name { get; set; }
        [Display(Name = "角色描述")]
        public string Description { get; set; }
        [Display(Name = "角色代码值")]
        public int Val { get; set; }
        public string[] RoleRightDisplay { get; set; }

        public override T ToEntity<TSource, T>()
        {
           if (typeof(RoleEntity).IsAssignableFrom(typeof(T)))
            {
                var entity = base.ToEntity<RoleViewModel, RoleEntity>();
                entity.RoleAccessRights = (from right in this.RoleRightDisplay
                                           select new RoleAccessRightEntity()
                                           {
                                               RoleId = entity.Id
                                              ,
                                               AccessRightId = int.Parse(right)
                                              ,
                                               Id = 0
                                           }).ToList();
                return entity as T;
            } else
            {
                return base.ToEntity<TSource,T>();
            }
               
        }
        public override T FromEntity<TSource, T>(TSource entity)
        {
            if (typeof(RoleEntity).IsAssignableFrom(typeof(TSource)))
            {
                var newEntity = entity as RoleEntity;
                var roleModel = base.FromEntity<RoleEntity, RoleViewModel>(newEntity);
                roleModel.RoleRightDisplay = (from right in newEntity.RoleAccessRights
                                              select right.AccessRightId.ToString()).ToArray();
                return roleModel as T;
            }
            else
            {
                return base.FromEntity<TSource, T>(entity);
            }
        }
    }

    public class UserRoleViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public IEnumerable<RoleViewModel> Roles { get; set; }

        [DisplayName("用户")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "customer")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public UserModel User { get; set; }
        public override T FromEntity<TSource, T>(TSource entity)
        {
           if (typeof(UserEntity).IsAssignableFrom(typeof(TSource)))
           {
               var newEntity = entity as UserEntity;
               var roleService = ServiceLocator.Current.Resolve<IUserRoleRepository>();
               User =Mapper.Map<UserEntity,UserModel>(newEntity);
               Roles = (from role in roleService.FindRolesByUserId(newEntity.Id)
                       select new RoleViewModel().FromEntity<RoleEntity, RoleViewModel>(role)).AsEnumerable();
               return this as T;
           }
            return default(T);
        }
        
    }
}
