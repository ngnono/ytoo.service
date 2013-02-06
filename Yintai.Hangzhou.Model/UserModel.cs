﻿using System.Collections.Generic;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model.Enums;
using System.Linq;

namespace Yintai.Hangzhou.Model
{
    public class UserModel : DomainModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }
        public int Status { get; set; }
        public int UserLevel { get; set; }

        public int Store_Id { get; set; }
        public int Region_Id { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public byte Gender { get; set; }

        public UserLevel Level
        {
            get { return (UserLevel)this.UserLevel; }
            set { this.UserLevel = (int)value; }
        }

        public StoreModel Store { get; set; }

        public RegionModel Region { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserAccountModel> Accounts { get; set; }

        public List<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public UserRole UserRole
        {
            get
            {
                if (UserRoles == null || UserRoles.Count == 0)
                {
                    return UserRole.None;
                }

                var ur = UserRole.User;
                foreach (var item in UserRoles)
                {
                    if (UserRole.User == item)
                    {
                        continue;
                    }

                    ur = ur | item;
                }

                return ur;
            }
        }

        /// <summary>
        /// 我喜欢
        /// </summary>
        public int ILikeCount { get; set; }

        /// <summary>
        /// 喜欢我
        /// </summary>
        public int LikeMeCount { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int FavorCount { get; set; }

        /// <summary>
        /// 优惠码数
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// 消费次数
        /// </summary>
        public int ConsumptionCount { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public int ShareCount { get; set; }
    }
}
