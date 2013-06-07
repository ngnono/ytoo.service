using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
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
        [Display(Name="用户代码")]
        public int Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 6)]
        [Display(Name = "用户名")]
        public string Name { get; set; }

        [Required]
        [StringLength(1024, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 0)]
        [Display(Name = "昵称")]
        public string Nickname { get; set; }

        [Display(Name = "最后登录时间")]
        [DataType(DataType.DateTime)]
        public DateTime LastLoginDate { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 0)]
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Required]
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

        [Required]
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

        public List<int> UserRoles { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public int UserRole
        {
            get
            {
                if (UserRoles == null || UserRoles.Count == 0)
                {
                    return (int)Yintai.Hangzhou.Model.Enums.UserRole.None;
                }

                var ur = (int)Yintai.Hangzhou.Model.Enums.UserRole.None;
                foreach (var item in UserRoles)
                {

                    ur = ur | item;
                }

                return ur;
            }
        }

        #endregion

        /// <summary>
        /// 我喜欢
        /// </summary>
        [Display(Name = "关注数")]
        public int ILikeCount { get; set; }

        /// <summary>
        /// 喜欢我
        /// </summary>
        [Display(Name = "被关注数")]
        public int LikeMeCount { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [Display(Name = "收藏数")]
        public int FavorCount { get; set; }

        /// <summary>
        /// 优惠码数
        /// </summary>
        [Display(Name = "优惠码数")]
        public int CouponCount { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        [Display(Name = "积分数")]
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
    public class CustomerListSearchOption
    {
        [Display(Name = "用户代码")]
        public int? PId { get; set; }
        [Display(Name = "用户名称")]
        public string Name { get; set; }
        [Display(Name = "手机号")]
        public string Mobile { get; set; }
        [Display(Name = "邮箱名")]
        public string Email { get; set; }
        [Display(Name = "排序")]
        public GenericOrder? OrderBy { get; set; }
      

    }
}
