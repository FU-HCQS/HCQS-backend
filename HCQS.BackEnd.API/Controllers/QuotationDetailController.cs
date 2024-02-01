using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
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
        private readonly IValidator<QuotationDetailDto> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;

        public QuotationDetailController(IQuotationDetailService service, IValidator<QuotationDetailDto> validator, HandleErrorValidator handleErrorValidator)
        {
            _service = service;
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
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

        //[HttpPost("create-quotation-detail")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        //public async Task<AppActionResult> CreateQuotationDetail(QuotationDetailDto quotationDetail)
        //{
        //    var result = await _validator.ValidateAsync(quotationDetail);
        //    if (!result.IsValid)
        //    {
        //        return _handleErrorValidator.HandleError(result);
        //    }
        //    return await _service.CreateQuotationDetail(quotationDetail);
        //}
        [HttpPost("create-list-quotation-detail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> CreateListQuotationDetail(List<QuotationDetailDto> quotationDetails)
        {
           foreach (var quotationDetail in quotationDetails)
            {
                var result = await _validator.ValidateAsync(quotationDetail);
                if (!result.IsValid)
                {
                    return _handleErrorValidator.HandleError(result);
                }
            }
            return await _service.CreateListQuotationDetail(quotationDetails);
        }

        [HttpPut("update-quotation-detail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateQuotationDetail(QuotationDetailDto quotationDetail)
        {
            var result = await _validator.ValidateAsync(quotationDetail);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
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