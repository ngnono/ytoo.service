using System;
using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.Filters;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Intime.OPC.WebApi.Controllers
{
    [APIExceptionFilter]
    [RoutePrefix("api/counters")]
    public class SectionController : BaseController
    {
        private readonly ISectionRepository _sectionRepository;

        public SectionController(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        private static Section CheckModel(Section model)
        {
            if (model == null)
            {
                return null;
            }

            //model.Location = model.Location ?? String.Empty;
            model.Name = model.Name ?? String.Empty;
            //model.StoreCode = model.StoreCode ?? String.Empty;
            model.ContactPhone = model.ContactPhone ?? String.Empty;
            model.SectionCode = model.SectionCode ?? String.Empty;


            return model;
        }

        [Route("all")]
        //[ActionName("all")]
        public IHttpActionResult GetAll()
        {
            int total;
            var model = _sectionRepository.GetPagedList(new PagerRequest(1, 100000, 100000), out total, null,
                                                        SectionSortOrder.Default);
            var dto = Mapper.Map<List<Section>, List<SectionDto>>(model);

            return RetrunHttpActionResult(dto);
        }

        [HttpGet]
        [Route("{id:int}")]

        //[ActionName("details")]
        public IHttpActionResult GetSection(int id)
        {
            var model = _sectionRepository.GetItem(id);


            var dto = Mapper.Map<Section, SectionDto>(model);

            return RetrunHttpActionResult(dto);
        }

        [NonAction]
        private IHttpActionResult GetSectionList(SectionFilter filter, [UserId] int userId)
        {
            if (filter == null)
            {
                filter = new SectionFilter();
            }

            filter.AuthUserId = userId;

            int total;

            var pagerRequest = new PagerRequest(filter.Page ?? 1, filter.PageSize ?? 0);

            var model = _sectionRepository.GetPagedList(pagerRequest, out total, filter,
                                                        filter.SortOrder ?? SectionSortOrder.Default);

            var dto = Mapper.Map<List<Section>, List<SectionDto>>(model);

            var pagerdto = new PagerInfo<SectionDto>(pagerRequest, total);
            pagerdto.Datas = dto;

            return RetrunHttpActionResult(pagerdto);
        }

        [HttpPost]
        [Route("list")]
        //[ActionName("list")]
        public IHttpActionResult PostSectionList([FromBody] SectionFilter filter, [UserId] int userId)
        {
            return GetSectionList(filter, userId);
        }

        // [ActionName("list")]/section?page=1,2&sortorder=0&prekeyname=11&storeid=1&status=2
        [Route("")]
        public IHttpActionResult GetList([FromUri]SectionFilter filter, [UserId] int userId)
        {
            return GetSectionList(filter, userId);
        }

        [ModelValidationFilter]
        [Route("{id:int}")]
        //[ActionName("details")] api/section/1
        public IHttpActionResult Put(int id, [FromBody]SectionDto dto, [UserId] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var item = _sectionRepository.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }

            var createDate = item.CreateDate;
            var createUser = item.CreateUser;


            dto.Id = id;
            Mapper.Map<SectionDto, Section>(dto, item);
            item.UpdateDate = DateTime.Now;
            item.UpdateUser = userId;
            item.CreateDate = createDate;
            item.CreateUser = createUser;

            item = CheckModel(item);
            ((IOPCRepository<int, Section>)_sectionRepository).Update(item);

            return GetSection(id);
        }

        //[ActionName("details")]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id, [UserId] int userId)
        {
            var item = _sectionRepository.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Status = -1;
            item.UpdateDate = DateTime.Now;
            item.UpdateUser = userId;

            //关系
            item.Brands = null;

            ((IOPCRepository<int, Section>)_sectionRepository).Update(item);

            return RetrunHttpActionResult("ok");
        }

        [ModelValidationFilter]
        [Route("")]
        //[ActionName("details")]
        public IHttpActionResult Post([FromBody]SectionDto dto, [UserId] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var model = Mapper.Map<SectionDto, Section>(dto);
            model.CreateDate = DateTime.Now;
            model.CreateUser = userId;
            model.UpdateDate = DateTime.Now;
            model.UpdateUser = userId;

            model.Status = dto.Status ?? 1;

            model = CheckModel(model);
            var item = _sectionRepository.Insert(model);

            return GetSection(item.Id);
        }
    }
}
