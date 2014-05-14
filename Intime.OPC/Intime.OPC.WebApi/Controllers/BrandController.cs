using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.Filters;

namespace Intime.OPC.WebApi.Controllers
{
    [RoutePrefix("api/brands")]
    public class BrandController : BaseController
    {
        private IBrandService _brandService;
        private IBrandRepository _brandRepository;
        public BrandController(IBrandService brandService, IBrandRepository brandRepository)
        {
            _brandService = brandService;
            _brandRepository = brandRepository;
        }

        [HttpPost]
        public IHttpActionResult GetAll([UserId] int? userId)
        {

            return DoFunction(() => _brandService.GetAll(), "获得品牌信息失败");

        }


        //[HttpGet]
        [Route("{id:int}")]

        //[ActionName("details")]
        public IHttpActionResult Get(int id)
        {
            var model = _brandRepository.GetItem(id);


            var dto = Mapper.Map<Brand, BrandDto>(model);

            return RetrunHttpActionResult(dto);
        }

        // [ActionName("list")]/brand?page=1,2&sortorder=0&prekeyname=11&storeid=1&status=2
        [Route("")]
        public IHttpActionResult GetList([FromUri]BrandFilter filter, [UserId] int userId)
        {
            int total;
            if (filter == null)
            {
                filter = new BrandFilter();
            }

            var pagerRequest = new PagerRequest(filter.Page ?? 1, filter.PageSize ?? 10);

            var model = _brandRepository.GetPagedList(pagerRequest, out total, filter, filter.SortOrder ?? BrandSortOrder.Default);

            var dto = Mapper.Map<List<Brand>, List<BrandDto>>(model);

            var pagerdto = new PagerInfo<BrandDto>(pagerRequest, total);
            pagerdto.Datas = dto;

            return RetrunHttpActionResult(pagerdto);
        }

        [ModelValidationFilter]
        [Route("{id:int}")]
        //[ActionName("details")] api/brand/1
        public IHttpActionResult Put(int id, [FromBody]BrandDto dto, [UserId] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var item = _brandRepository.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }

            var createDate = item.CreatedDate;
            var createUser = item.CreatedUser;

            dto.Id = id;
            item = Mapper.Map<BrandDto, Brand>(dto, item);
            item.UpdatedDate = DateTime.Now;
            item.UpdatedUser = userId;
            item.CreatedDate = createDate;
            item.CreatedUser = createUser;


            ((IOPCRepository<int, Brand>)_brandRepository).Update(item);

            return Get(id);
        }

        //[ActionName("details")]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id, [UserId] int userId)
        {
            var item = _brandRepository.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Status = -1;
            item.UpdatedDate = DateTime.Now;
            item.UpdatedUser = userId;

            ((IOPCRepository<int, Brand>)_brandRepository).Update(item);

            return RetrunHttpActionResult("OK");
        }

        [ModelValidationFilter]
        [Route("")]
        //[ActionName("details")]
        public IHttpActionResult Post([FromBody]BrandDto dto, [UserId] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Brand model = null;
            try
            {
                model = Mapper.Map<BrandDto, Brand>(dto);
            }
            catch (Exception ex)
            {

                throw;
            }

            model.UpdatedDate = DateTime.Now;
            model.UpdatedUser = userId;
            model.CreatedDate = DateTime.Now;
            model.CreatedUser = userId;

            var item = _brandRepository.Insert(model);

            return Get(item.Id);
        }
    }
}
