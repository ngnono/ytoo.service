﻿using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage
{
    //test
    //liuyh 2014-1-1
    [ModuleExport(typeof (TransManageModule))]
    public class TransManageModule : IModule
    {
        [Import] public IRegionManager RegionManager;

        public void Initialize()
        {
            // this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(NavigationItemView));
            Mapper.CreateMap<OPC_Comment, OPC_SaleComment>(e =>
                new OPC_SaleComment
                {
                    Id = e.Id,
                    SaleOrderNo = e.RelationId,
                    Content = e.Content,
                    CreateDate = e.CreateDate,
                    CreateUser = e.CreateUser
                }
                );

            Mapper.CreateMap<OPC_SaleComment, OPC_Comment>(e =>
                new OPC_Comment
                {
                    Id = e.Id,
                    RelationId = e.SaleOrderNo,
                    Content = e.Content,
                    CreateDate = e.CreateDate,
                    CreateUser = e.CreateUser
                }
                );


            Mapper.CreateMap<OPC_Comment, OPC_SaleDetailsComment>(e =>
                new OPC_SaleDetailsComment
                {
                    Id = e.Id,
                    SaleDetailId = e.RelationId,
                    Content = e.Content,
                    CreateDate = e.CreateDate,
                    CreateUser = e.CreateUser
                }
                );

            Mapper.CreateMap<OPC_SaleDetailsComment, OPC_Comment>(e =>
                new OPC_Comment
                {
                    Id = e.Id,
                    RelationId = e.SaleDetailId,
                    Content = e.Content,
                    CreateDate = e.CreateDate,
                    CreateUser = e.CreateUser
                }
                );
        }
    }
}