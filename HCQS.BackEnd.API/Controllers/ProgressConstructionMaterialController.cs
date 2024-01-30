using FluentValidation;
using HCQS.BackEnd.Common.Dto.BaseRequest;
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
    [Route("progress-construction-material")]
    [ApiController]
    public class ProgressConstructionMaterialController : Controller
    {
        private readonly IValidator<ProgressConstructionMaterialRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IProgressConstructionMaterialService _progressConstructionMaterialService;
        public ProgressConstructionMaterialController(IValidator<ProgressConstructionMaterialRequest> validator, HandleErrorValidator handleErrorValidator, IProgressConstructionMaterialService progressConstructionMaterialService)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _progressConstructionMaterialService = progressConstructionMaterialService;
        }

        [HttpGet("get-progress-construction-material-by-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetProgressConstructionMaterialById(Guid Id)
        {
            return await _progressConstructionMaterialService.GetProgressConstructionMaterialById(Id);
        }

        [HttpPut("update-progress-construction-material")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateProgressConstructionMaterial(ProgressConstructionMaterialRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _progressConstructionMaterialService.UpdateProgressConstructionMaterial(request);
        }

        [HttpPost("get-all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _progressConstructionMaterialService.GetAll(pageIndex, pageSize, sortInfos);
        }

        [HttpPost("get-all-by-quotation-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAllByQuotationId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _progressConstructionMaterialService.GetAllByQuotationId(Id, pageIndex, pageSize, sortInfos);
        }

        [HttpPost("get-all-by-quotation-detail-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAllByQuotationDetailId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _progressConstructionMaterialService.GetAllByQuotationDetailId(Id, pageIndex, pageSize, sortInfos);
        }

        [HttpPost("create-progress-construction-material")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> CreateProgressConstructionMaterial(List<ProgressConstructionMaterialRequest> ProgressConstructionMaterialRequests)
        {
            foreach (var request in ProgressConstructionMaterialRequests)
            {
                var result = await _validator.ValidateAsync(request);
                if (!result.IsValid)
                {
                    return _handleErrorValidator.HandleError(result);
                }
            }
            return await _progressConstructionMaterialService.CreateProgressConstructionMaterial(ProgressConstructionMaterialRequests);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-progress-construction-material-by-id/{Id}")]
        public async Task<AppActionResult> DeleteProgressConstructionMaterialById(Guid Id)
        {
            return await _progressConstructionMaterialService.DeleteProgressConstructionMaterialById(Id);
        }

    }
}
