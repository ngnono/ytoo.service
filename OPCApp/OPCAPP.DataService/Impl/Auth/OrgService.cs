﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof (IOrgService))]
    public class OrgService : IOrgService
    {
        public string Name { get; set; }

        public ResultMsg Add(OPC_OrgInfo org)
        {
            try
            {
                if (org == null)
                {
                    return new ResultMsg {IsSuccess = false, Msg = "增加错误"};
                }

                OPC_OrgInfo ent = RestClient.PostReturnModel("org/addOrg", org);
                return new ResultMsg {IsSuccess = ent != null, Msg = "保存成功", Data = ent};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Edit(OPC_OrgInfo org)
        {
            try
            {
                bool bFalg = RestClient.Put("org/updateOrg", org);
                return new ResultMsg {IsSuccess = bFalg, Msg = "保存成功"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }

        public ResultMsg Delete(OPC_OrgInfo org)
        {
            try
            {
                var oo = new {orgInfoId = org.Id};
                bool bFalg = RestClient.Put("org/deleteorg", org.Id);
                return new ResultMsg {IsSuccess = bFalg, Msg = "删除错误"};
            }
            catch (Exception ex)
            {
                return new ResultMsg {IsSuccess = false, Msg = "保存失败"};
            }
        }


        public IList<OPC_OrgInfo> Search()
        {
            try
            {
                IList<OPC_OrgInfo> lst = RestClient.Get<OPC_OrgInfo>("org/getall", "");
                return lst;
            }
            catch (Exception ex)
            {
                return new List<OPC_OrgInfo>();
            }
        }


        PageResult<OPC_OrgInfo> IBaseDataService<OPC_OrgInfo>.Search(IDictionary<string, object> iDicFilter)
        {
            throw new NotImplementedException();
        }
    }
}