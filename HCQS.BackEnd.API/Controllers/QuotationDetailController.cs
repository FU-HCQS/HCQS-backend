using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("quotation-detail")]
    [ApiController]
    public class QuotationDetailController : ControllerBase
    {
        private IQuotationDetailService _service;

        public QuotationDetailController(IQuotationDetailService service)
        {
            _service = service;
        }

        [HttpGet("get-quotation-detail-by-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> GetQuotationDetailById(Guid id)
        {
            return await _service.GetQuotationDetailById(id);
        }

        [HttpGet("get-quotation-detail-by-quotation-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.ALL)]

        public async Task<AppActionResult> GetAllQuotationDetailByQuotationId(Guid id)
        {
            return await _service.GetAllQuotationDetailByQuotationId(id);
        }

        [HttpPost("create-quotation-detail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> CreateQuotationDetail(QuotationDetailDto quotationDetail)
        {
            return await _service.CreateQuotationDetail(quotationDetail);
        }

        [HttpPut("update-quotation-detail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]

        public async Task<AppActionResult> UpdateQuotationDetail(QuotationDetailDto quotationDetail)
        {
            return await _service.UpdateQuotationDetail(quotationDetail);
        }

        [HttpDelete("delete-quotation-detail-by-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteQuotationDetail(Guid id)
        {
            return await _service.DeleteQuotationDetail(id);
        }
    }
}