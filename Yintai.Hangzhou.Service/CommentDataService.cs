using System;
using System.Linq;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.Comment;
using Yintai.Hangzhou.Contract.DTO.Request.Comment;
using Yintai.Hangzhou.Contract.DTO.Response.Comment;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public class CommentDataService : BaseService, ICommentDataService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IRemindService _remindService;

        public CommentDataService(ICommentRepository commentRepository, IRemindService remindService)
        {
            this._commentRepository = commentRepository;
            this._remindService = remindService;
        }

        public ExecuteResult<CommentCollectionResponse> GetList(CommentListRequest request)
        {
            int totalCount;
            var data = this._commentRepository.GetPagedList(request.PagerRequest.PageIndex, request.PagerRequest.PageSize,
                                                 out totalCount, request.SortOrder, request.Timestamp, request.SourceId, request.SType);

            var result = new ExecuteResult<CommentCollectionResponse>();
            var response = new CommentCollectionResponse(request.PagerRequest, totalCount)
                {
                    Comments = MappingManager.CommentInfoResponseMapping(data).ToList()
                };

            result.Data = response;

            return result;
        }

        public ExecuteResult<CommentInfoResponse> Create(CommentCreateRequest request)
        {
            var entity = this._commentRepository.Insert(new CommentEntity
                {
                    Content = request.Content,
                    CreatedDate = DateTime.Now,
                    CreatedUser = request.AuthUid,
                    ReplyUser = request.ReplyUser,
                    SourceId = request.SourceId,
                    SourceType = (int)request.SType,
                    Status = 1,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = request.AuthUid,
                    User_Id = request.AuthUid
                });

            //插入一个提醒
            _remindService.Insert(new RemindEntity
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = request.AuthUid,
                    IsRemind = false,
                    RemindUser = request.AuthUid,
                    SourceId = entity.Id,
                    Stauts = (int)DataStatus.Default,
                    Type = (int)SourceType.Comment,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = request.AuthUid,
                    User_Id = entity.ReplyUser
                });


            return new ExecuteResult<CommentInfoResponse>(MappingManager.CommentInfoResponseMapping(entity));
        }

        public ExecuteResult<CommentInfoResponse> Detail(CommentDetailRequest request)
        {
            var entity = this._commentRepository.GetItem(request.CommentId);

            return new ExecuteResult<CommentInfoResponse>(MappingManager.CommentInfoResponseMapping(entity));
        }

        public ExecuteResult<CommentInfoResponse> Update(CommentUpdateRequest request)
        {
            var entity = this._commentRepository.GetItem(request.CommentId);
            if (entity == null)
            {
                return new ExecuteResult<CommentInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "评论不存在" };
            }

            if (entity.User_Id != request.AuthUid)
            {
                return new ExecuteResult<CommentInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "您没有权限修改其他人的评论" };
            }

            entity.Content = request.Content;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = request.AuthUid;

            this._commentRepository.Update(entity);

            return new ExecuteResult<CommentInfoResponse>(MappingManager.CommentInfoResponseMapping(entity));
        }

        public ExecuteResult<CommentInfoResponse> Destroy(CommentDestroyRequest request)
        {
            var entity = this._commentRepository.GetItem(request.CommentId);
            if (entity == null)
            {
                return new ExecuteResult<CommentInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "评论不存在" };
            }

            if (entity.User_Id != request.AuthUid)
            {
                return new ExecuteResult<CommentInfoResponse>(null) { StatusCode = StatusCode.ClientError, Message = "您没有权限删除其他人的评论" };
            }

            this._commentRepository.Delete(entity);

            return new ExecuteResult<CommentInfoResponse>(MappingManager.CommentInfoResponseMapping(entity));
        }

        public ExecuteResult<CommentCollectionResponse> GetListRefresh(CommentRefreshRequest request)
        {
            var data = this._commentRepository.GetList(request.PagerRequest.PageSize, request.SortOrder,
                                                           request.Timestamp, request.SourceId, request.SType);

            var result = new ExecuteResult<CommentCollectionResponse>();
            var response = new CommentCollectionResponse(request.PagerRequest)
            {
                Comments = MappingManager.CommentInfoResponseMapping(data).ToList()
            };
            result.Data = response;

            return result;
        }
    }
}
