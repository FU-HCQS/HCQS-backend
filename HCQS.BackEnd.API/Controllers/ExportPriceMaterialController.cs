using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("export-price-material")]
    [ApiController]
    public class ExportPriceMaterialController : Controller
    {
        public IValidator<ExportPriceMaterialRequest> _validator;
        public HandleErrorValidator _handleErrorValidator;
        public IExportPriceMaterialService _service;

        public ExportPriceMaterialController(IValidator<ExportPriceMaterialRequest> validator, HandleErrorValidator errorValidator, IExportPriceMaterialService service)
        {
            _validator = validator;
            _handleErrorValidator = errorValidator;
            _service = service;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-export-price-material")]
        public async Task<AppActionResult> CreateExportPriceMaterial(ExportPriceMaterialRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _service.CreateExportPriceMaterial(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("update-export-price-material")]
        public async Task<AppActionResult> UpdateExportPriceMaterial(ExportPriceMaterialRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _service.UpdateExportPriceMaterial(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("import-export-price-material-from-excelsheet")]
        public async Task<IActionResult> ImportExportPriceMaterialFromExcel(IFormFile file)
        {
            return await _service.UploadExportPriceMaterialWithExcelFile(file);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-export-price-material-by-id/{id}")]
        public async Task<AppActionResult> DeleteExportPriceMaterialById(Guid id)
        {
            return await _service.DeleteExportPriceMaterialById(id);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-export-price-material-by-id/{id}")]
        public async Task<AppActionResult> GetExportPriceMaterialById(Guid id)
        {
            return await _service.GetExportPriceMaterialById(id);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-all-export-price-material")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _service.GetAll(pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-latest-export-price-material")]
        public async Task<AppActionResult> GetLatest(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _service.GetLatestPrice(pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-export-price-material-template")]
        public async Task<IActionResult> GetExportPriceMaterialTemplate()
        {
            return await _service.GetExportPriceMaterialTemplate();
        }
    }
}