using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using com.intime.fashion.webapi.domain.Request.Assistant;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public partial class AssistantController
    {
        /// <summary>
        /// 导购开店申请
        /// // todo 未做电话号码校验
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult Apply(DaogouApplicationRequest request)
        {
            if (request.AuthUser == null || request.AuthUser.Id == 0)
            {
                return this.RenderError(r => r.Message = "未登录的用户");
            }

            var associate = _associateRepo.Get(x => x.UserId == request.AuthUser.Id).FirstOrDefault();


            if (associate != null)
            {
                if (request.AuthUser.Level == UserLevel.DaoGou)
                {
                    return this.RenderError(r => r.Message = "用户已是迷你银导购，不能重复申请");
                }
            }

            var history =
                _inviteCodeRequestRepo.Get(x => x.UserId == request.AuthUser.Id && x.Status == 1 && x.Approved == null)
                    .FirstOrDefault();
            if (history != null)
            {
                return this.RenderError(r => r.Message = "您已经申请了，不能重复申请");
            }

            if (string.IsNullOrEmpty(request.Phone))
            {
                return this.RenderError(r => r.Message = "电话号码为空!");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                return this.RenderError(r => r.Message = "姓名为空!");
            }

            if (string.IsNullOrEmpty(request.SectionCode))
            {
                return this.RenderError(r => r.Message = "专柜编码为空!");
            }

            if (request.StoreId <= 0)
            {
                return this.RenderError(r => r.Message = "所属门店为空!");
            }

            if (!_sectionRepo.Contains(x => x.StoreId == request.StoreId && request.SectionCode == x.SectionCode))
            {
                return this.RenderError(r => r.Message = "无效的专柜编码!");
            }

            _inviteCodeRequestRepo.Insert(new IMS_InviteCodeRequestEntity()
            {
                ContactMobile = request.Phone,
                CreateDate = DateTime.Now,
                CreateUser = request.AuthUser.Id,
                Name = request.Name,
                OperatorCode = request.OperatorCode,
                SectionCode = request.SectionCode,
                SectionName = request.SectionName,
                StoreId = request.StoreId,
                RequestType = request.ApplyType,
                UserId = request.AuthUser.Id,
                UpdateDate = DateTime.Now,
                UpdateUser = request.AuthUser.Id,
                Status = (int)DataStatus.Normal,
            });
            return this.RenderSuccess<dynamic>(null);
        }
    }
}
