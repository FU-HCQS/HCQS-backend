using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("supplier")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly IValidator<SupplierRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private ISupplierService _supplierService;

        public SupplierController(IValidator<SupplierRequest> validator, HandleErrorValidator handleErrorValidator, ISupplierService service)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _supplierService = service;
        }

        [HttpPost("create-supplier")]
        public async Task<AppActionResult> CreateSupplier(SupplierRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _supplierService.CreateSupplier(request);
        }

        [HttpPut("update-supplier")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateSupplier(SupplierRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _supplierService.UpdateSupplier(request);
        }

        [HttpPost("import-supplier-from-excelsheet")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<IActionResult> ImportSupplierFromExcelsheet(IFormFile request)
        {
            return await _supplierService.ImportSupplierWithExcelFile(request);
        }

        [HttpDelete("delete-supplier-by-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteSupplierById(Guid Id)
        {
            return await _supplierService.DeleteSupplierById(Id);
        }

        [HttpGet("get-supplier-by-id/{Id}")]
        public async Task<AppActionResult> GetSupplierById(Guid Id)
        {
            return await _supplierService.GetSupplierById(Id);
        }

        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierService.GetAll(pageIndex, pageSize, sortInfos);
        }

        [HttpGet("get-supplier-template")]
        public async Task<IActionResult> GetSupplierTemplate()
        {
            return await _supplierService.GetSupplierTemplate();
        }
    }
}