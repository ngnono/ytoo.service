﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Repository.Impl;
using Intime.OPC.Repository.Support;
using Intime.OPC.WebApi.Controllers;
using NUnit.Framework;

namespace Intime.OPC.WebApi.Test.ControllerTest
{
    public class SectionControllerTest : BaseControllerTest
    {
        private SectionController _controller;

        public SectionController GetController()
        {
            var sectionRepository = new SectionRepository();

            _controller = new SectionController(sectionRepository);

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
        public void GetTest([Values(883)]int id)
        {
            _controller.Request.Method = HttpMethod.Get;
            var actual = _controller.GetSection(id) as OkNegotiatedContentResult<SectionDto>;
            var sectionDto = actual.Content;

            Assert.IsNotNull(actual);
            Assert.IsTrue(sectionDto != null);
            Assert.IsNotNull(actual.Content.Store);
        }


        [Test()]
        public void GetListTest_1()
        {
            _controller.Request.Method = HttpMethod.Get;
            var actual = _controller.GetList(new SectionFilter
            {
                Page = 1,
                PageSize = 10
            }, 0) as OkNegotiatedContentResult<PagerInfo<SectionDto>>;

            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetListTest_2([Values(1000006)]int? BrandId)
        {
            _controller.Request.Method = HttpMethod.Get;
            var actual = _controller.GetList(new SectionFilter
            {
                Page = 1,
                PageSize = 20,
                BrandId = BrandId
            }, 0) as OkNegotiatedContentResult<PagerInfo<SectionDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.Datas.Count > 0);
        }

        [Test()]
        public void GetListTest_3([Values("梦")]string namePrefix)
        {
            _controller.Request.Method = HttpMethod.Get;

            var actual = _controller.GetList(new SectionFilter
            {
                Page = 1,
                PageSize = 20,
                NamePrefix = namePrefix,
            }, 0) as OkNegotiatedContentResult<PagerInfo<SectionDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.Datas.Count > 0);
        }

        [Test()]
        public void GetListTest_4([Values("小骆驼")]string name)
        {
            _controller.Request.Method = HttpMethod.Get;

            var actual = _controller.GetList(new SectionFilter
            {
                Page = 1,
                PageSize = 20,
                Name = name
            }, 0) as OkNegotiatedContentResult<PagerInfo<SectionDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.Datas.Count > 0);
        }

        [Test()]
        public void Post()
        {
            _controller.Request.Method = HttpMethod.Post;

            var actual = _controller.Post(new SectionDto
            {
                Name = "Test_001",
                Code = "asdfq2@$a",
                ContactPhone = "110",
                Status  = 1,
                StoreId = 3,
                Repealed = true

            }, 0) as OkNegotiatedContentResult<SectionDto>;
            var sectionDto = actual.Content;

            Assert.IsNotNull(actual);
            Assert.IsTrue(sectionDto != null);
        }

        [Test()]
        public void Put([Values(883)]int id)
        {
            _controller.Request.Method = HttpMethod.Put;

            var actual = _controller.Put(id, new SectionDto
            {
                Name = "Test_002",
                Status = 1,
                Id = id,
                Code = "asdf9876",
                StoreId = 4,
                Repealed = false,
                Brands = new List<BrandDto>()
                {
                    new BrandDto()
                    {
                        Id=1000265,
                        Name = "kelamiti"
                    },
                                        new BrandDto()
                    {
                        Id=1000266,
                        Name = "adl"
                    },
                }


            }, 0) as OkNegotiatedContentResult<SectionDto>;

            Assert.IsNotNull(actual);
        }

        [Test]
        public void Delete([Values(879)]int id)
        {
            _controller.Request.Method = HttpMethod.Delete;

            var actual = _controller.Delete(id, 0) as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(actual);
        }
    }
}
