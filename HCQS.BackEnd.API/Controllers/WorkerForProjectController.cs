using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("worker-for-project")]
    [ApiController]
    public class WorkerForProjectController : ControllerBase
    {
        private IWorkerForProjectService _service;

        public WorkerForProjectController(IWorkerForProjectService service)
        {
            _service = service;
        }

        [HttpGet("get-list-worker-by-quotation-id/{id}")]
        public async Task<AppActionResult> GetListWorkerByQuotationId(Guid id)
        {
            return await _service.GetListWorkerByQuotationId(id);
        }
    }
}
