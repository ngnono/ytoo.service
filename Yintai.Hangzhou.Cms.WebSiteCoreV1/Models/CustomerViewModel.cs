using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class CustomerCollectionViewModel : PagerInfo, IViewModel
    {
        public CustomerCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public CustomerCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<CustomerViewModel> Customers { get; set; }
    }

    public class CustomerViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 6)]
        [Display(Name = "用户名")]
        public string Name { get; set; }

        [Required]
        [StringLength(1024, MinimumLength = 6)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [StringLength(32, MinimumLength = 0)]
        [Display(Name = "昵称")]
        public string Nickname { get; set; }

        [Display(Name = "最后登录时间")]
        [DataType(DataType.DateTime)]
        public DateTime LastLoginDate { get; set; }

        [StringLength(32, MinimumLength = 0)]
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [StringLength(64, MinimumLength = 0)]
        [Display(Name = "邮箱")]
        public string EMail { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "用户等级")]
        public int UserLevel { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "所属店铺Id")]
        public int Store_Id { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "地区Id")]
        public int Region_Id { get; set; }

        [Display(Name = "头像")]
        [StringLength(1024, MinimumLength = 0)]
        public string Logo { get; set; }

        [Display(Name = "说明")]
        [StringLength(1024, MinimumLength = 0)]
        public string Description { get; set; }

        [Range(0, 256)]
        [Display(Name = "性别")]
        public byte Gender { get; set; }

        #region 需要关联的属性

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

        #endregion

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


        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "创建人")]
        public int CreatedUser { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime CreatedDate { get; set; }
        [Display(Name = "修改日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime UpdatedDate { get; set; }
        [Display(Name = "修改人")]
        public int UpdatedUser { get; set; }
    }
}
