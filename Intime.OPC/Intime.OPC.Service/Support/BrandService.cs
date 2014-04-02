using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class BrandService : BaseService<Brand>, IBrandService
    {
        public BrandService(IBrandRepository brandRepository) : base(brandRepository)
        {
        }

        public PageResult<Brand> GetAll(int pageIndex, int pageSize = 20)
        {
            return _repository.GetAll(pageIndex, pageSize);
        }
    }
}