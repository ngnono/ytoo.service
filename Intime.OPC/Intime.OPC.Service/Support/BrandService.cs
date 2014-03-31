using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class BrandService : BaseService, IBrandService
    {
        private IBrandRepository _brandRepository;
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public PageResult<Brand> GetAll(int pageIndex, int pageSize = 20)
        {
            return _brandRepository.GetAll(pageIndex,pageSize);
        }
    }
}
