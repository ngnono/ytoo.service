
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IBrandService : IService
    {
        PageResult<Brand> GetAll(int pageIndex, int pageSize = 20);
    }
}
