using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using PagerRequest = Intime.OPC.Domain.PagerRequest;

namespace Intime.OPC.Service.Support
{
    public class StoreService : BaseService<Store>, IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository repository)
            : base(repository)
        {
            _storeRepository = repository;
        }

        public IList<Store> GetAll()
        {
            return _repository.GetAll(0, 1000).Result;
        }

        public PagerInfo<StoreDto> GetPagedList(StoreRequest storeRequest)
        {
            var pagerequest = new PagerRequest(storeRequest.Page ?? 1, storeRequest.PageSize ?? 10);

            var storeFilter = AutoMapper.Mapper.Map<StoreRequest, StoreFilter>(storeRequest);

            int totalCount;
            var list = _storeRepository.GetPagedList(pagerequest, out totalCount, storeFilter);

            var dto = AutoMapper.Mapper.Map<List<Store>, List<StoreDto>>(list);
            var page = new PagerInfo<StoreDto>(pagerequest) { Datas = dto };

            return page;
        }
    }
}
