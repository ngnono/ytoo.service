using System;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Contract.Apns;
using Yintai.Hangzhou.Contract.DTO.Request.Device;
using Yintai.Hangzhou.Contract.DTO.Response.Device;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Service
{
    public class ApnsDataService : BaseService, IApnsDataService
    {
        //private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IDeviceLogsRepository _deviceLogsRepository;

        public ApnsDataService( IDeviceLogsRepository deviceLogsRepository)
        {
            //this._deviceTokenRepository = deviceTokenRepository;
            this._deviceLogsRepository = deviceLogsRepository;
        }

        public ExecuteResult<DeviceLogInfoResponse> Register(DeviceRegisterRequest request)
        {
            //记LOG
            var entity = this._deviceLogsRepository.Insert(new DeviceLogEntity
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = request.AuthUid,
                    DeviceToken = request.DeviceToken,
                    DeviceUid = request.Uid,
                    Latitude = Convert.ToDecimal(request.Lat),
                    Longitude = Convert.ToDecimal(request.Lng),
                    Status = 1,
                    Type = 1,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = request.AuthUid,
                    User_Id = request.AuthUid
                });

            ////token表
            //var entity = this._deviceTokenRepository.GetItemByUserIdToken(request.AuthUid, request.DeviceToken);

            //if (entity == null)
            //{
            //    //注册
            //    entity = this._deviceTokenRepository.Insert(new DeviceTokenEntity
            //        {
            //            CreatedDate = DateTime.Now,
            //            CreatedUser = request.AuthUid,
            //            Status = 1,
            //            Token = request.DeviceToken,
            //            Type = 1,
            //            UpdatedDate = DateTime.Now,
            //            UpdatedUser = request.AuthUid,
            //            User_Id = request.AuthUid
            //        });
            //}
            //else
            //{
            //    entity.UpdatedDate = DateTime.Now;
            //    entity.UpdatedUser = request.AuthUid;

            //    this._deviceTokenRepository.Update(entity);
            //    //if (entity.User_Id != request.AuthUid)
            //    //{
            //    //    //问题
            //    //    Logger.Warn(String.Format("在注册Apns时，出现了不同用户相同的token,请求的User{0},已注册的用户{1},Token={2},Id={3}", request.AuthUid, entity.User_Id, entity.Token, entity.Id));

            //    //    return new ExecuteResult<ApnsInfoResponse>(null) { StatusCode = StatusCode.ClientError, "devicetoken重复" };
            //    //}
            //    //允许

            //    //if (entity.Token != request.DeviceToken)
            //    //{
            //    //    //问题
            //    //    Logger.Warn(String.Format("在注册Apns时，出现了同用户不同token的情况，请求的用户{0},已注册的token={1},当前的token={2},已注册id={3}", request.AuthUid,entity.Token,request.DeviceToken));

            //    //}
            //}

            return new ExecuteResult<DeviceLogInfoResponse> { Data = MappingManager.DeviceLogInfoResponseMapping(entity) };
        }
    }
}
