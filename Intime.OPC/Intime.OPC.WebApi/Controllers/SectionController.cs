using System.Web.Http;
using Intime.OPC.Repository;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class SectionController : BaseController
    {
        private readonly ISectionRepository _sectionRepository;

        public SectionController(ISectionRepository repository)
        {
            _sectionRepository = repository;
        }

        [HttpPost]
        public IHttpActionResult GetAll()
        {
            return DoFunction(() => _sectionRepository.GetAll(0, 10000).Result, "获得专柜信息失败");
        }
    }
}