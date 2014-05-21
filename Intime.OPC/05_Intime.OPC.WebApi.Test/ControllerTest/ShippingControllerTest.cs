﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Repository;
using Intime.OPC.Repository.Impl;
using Intime.OPC.Repository.Support;
using Intime.OPC.Service;
using Intime.OPC.Service.Contract;
using Intime.OPC.Service.Impl;
using Intime.OPC.Service.Support;
using Intime.OPC.WebApi.App_Start;
using Intime.OPC.WebApi.Controllers;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using NUnit.Framework;
using IOrderService = Intime.OPC.Service.IOrderService;
using OrderService = Intime.OPC.Service.Support.OrderService;

namespace Intime.OPC.WebApi.Test.ControllerTest
{
    [TestFixture]
    public class ShippingControllerTest : BaseControllerTest
    {
        private ShippingController _controller;

        

        public ShippingController GetController()
        {
            _controller = new ShippingController( new ShippingOrderRepository(), new SalesOrderRepository(), new OrderRepository(),new ShipViaRepository());

            _controller.Request = new HttpRequestMessage();
            _controller.Request.SetConfiguration(new HttpConfiguration());

            return _controller;
        }

        [SetUp]
        public void TestInit()
        {
            _controller = GetController();
        }

        [TearDown]
        public void TestCleanUp()
        {
            _controller = null;
        }

        

        [Test()]
        public void GetListTest_1()
        {
            _controller.Request.Method = HttpMethod.Get;

            var storeIds = new List<int>()
            {
                21,23
            };
            var actual = _controller.GetList(new GetShippingSaleOrderRequest
            {
                Page = 1,
                PageSize = 10,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddYears(-1),
                Status = EnumSaleOrderStatus.ShipInStorage
            }, 28, new UserProfile { IsSystem = true, StoreIds = storeIds }) as OkNegotiatedContentResult<PagerInfo<ShippingSaleDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.TotalCount > 0);
        }

        [Test()]
        public void GetListTest_2()
        {
            _controller.Request.Method = HttpMethod.Get;

            var storeIds = new List<int>()
            {
                200,500
            };

            var actual = _controller.GetList(new GetShippingSaleOrderRequest
            {
                Page = 1,
                PageSize = 10,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddYears(-1),
                Status = EnumSaleOrderStatus.ShipInStorage
            }, 28, new UserProfile { IsSystem = false, StoreIds = storeIds }) as OkNegotiatedContentResult<PagerInfo<ShippingSaleDto>>;

            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetListTest_3()
        {
            _controller.Request.Method = HttpMethod.Get;

            var storeIds = new List<int>()
            {
                21,23
            };
            var actual = _controller.GetList(new GetShippingSaleOrderRequest
            {
                Page = 1,
                PageSize = 10,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddYears(-1),
                Status = EnumSaleOrderStatus.ShipInStorage
            }, 28, new UserProfile { IsSystem = false, StoreIds = storeIds }) as OkNegotiatedContentResult<PagerInfo<ShippingSaleDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.TotalCount > 0);
        }

        [Test()]
        public void PutPrintTest([Values(1,2)]int? type)
        {
            _controller.Request.Method = HttpMethod.Put;

            var storeIds = new List<int>()
            {
                21,23
            };
            var actual = _controller.PutPrint(4,new DeliveryOrderPrintRequest
            {
                Type = type,
                Times = 1

            }, 28, new UserProfile { IsSystem = false, StoreIds = storeIds }) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(actual);
        }
    }
}
