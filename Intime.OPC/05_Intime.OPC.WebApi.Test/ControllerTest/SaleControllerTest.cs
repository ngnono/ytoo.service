using System;
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
using NUnit.Framework;
using IOrderService = Intime.OPC.Service.IOrderService;
using OrderService = Intime.OPC.Service.Support.OrderService;

namespace Intime.OPC.WebApi.Test.ControllerTest
{
    [TestFixture]
    public class SaleControllerTest : BaseControllerTest
    {
        private SaleController _controller;

        public SaleController GetController()
        {
            ISaleRepository saleRepository = new SaleRepository();
            ISaleRemarkRepository saleRemarkRepository = new SaleRemarkRepository();
            IAccountRepository accountRepository = new AccountRepository();
            IOrgInfoRepository orgInfoRepository = new OrgInfoRepository();
            IRoleUserRepository roleUserRepository = new RoleUserRepository();
            ISectionRepository sectionRepository = new SectionRepository();
            IStoreRepository storeRepository = new StoreRepository();
            IAccountService accountService = new AccountService(accountRepository, orgInfoRepository, roleUserRepository, sectionRepository, storeRepository);
            ISaleService saleService = new SaleService(saleRepository, saleRemarkRepository, accountService);
            IBrandRepository brandRepository = new BrandRepository();

            IShippingSaleRepository shippingSaleRepository = new ShippingSaleRepository();
            IOrderRepository orderRepository = new OrderRepository();
            IOrderRemarkRepository orderRemarkRepository = new OrderRemarkRepository();
            IOrderItemRepository orderItemRepository = new OrderItemRepository();
            ISaleDetailRepository saleDetailRepository = new SaleDetailRepository();
            ISaleRMARepository saleRmaRepository = new SaleRMARepository();
            IOrderService orderService = new OrderService(orderRepository, orderRemarkRepository, orderItemRepository, brandRepository, accountService, saleDetailRepository, saleRmaRepository);
            IShippingSaleService shippingSaleService = new ShippingSaleService(shippingSaleRepository, orderRepository, saleRmaRepository, accountService);
            ISaleOrderRepository saleOrderRepository = new SaleOrderRepository();
            ISaleOrderService saleOrderService = new SaleOrderService(saleOrderRepository);
            _controller = new SaleController(saleService, shippingSaleService, saleOrderService);

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
            var actual = _controller.GetList(new SaleOrderQueryRequest
            {
                Page = 1,
                PageSize = 10,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddYears(-1),
                Status = EnumSaleOrderStatus.ShipInStorage
            }, 28) as OkNegotiatedContentResult<PagerInfo<SaleDto>>;

            Assert.IsNotNull(actual);
        }

    }
}
