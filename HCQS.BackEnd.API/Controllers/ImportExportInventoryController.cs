using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("import-export-inventory")]
    [ApiController]
    public class ImportExportInventoryController : Controller
    {
        private readonly IValidator<ImportExportInventoryRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IImportExportInventoryHistoryService _importExportInventoryHistoryService;

        public ImportExportInventoryController(IValidator<ImportExportInventoryRequest> validator, IImportExportInventoryHistoryService importExportInventoryHistoryService, HandleErrorValidator handleErrorValidator)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _importExportInventoryHistoryService = importExportInventoryHistoryService;
        }

        [HttpPost("get-all")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _importExportInventoryHistoryService.GetAll(pageIndex, pageSize, sortInfos);
        }

        [HttpPost("get-all-import")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAllImport(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _importExportInventoryHistoryService.GetAllImport(pageIndex, pageSize, sortInfos);
        }

        [HttpPost("get-all-export")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> GetAllExport(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _importExportInventoryHistoryService.GetAllExport(pageIndex, pageSize, sortInfos);
        }

        [HttpPost("import-material")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> ImportMaterial(List<ImportExportInventoryRequest> ImportExportInventoryRequests)
        {
            foreach (var request in ImportExportInventoryRequests)
            {
                var result = await _validator.ValidateAsync(request);
                if (!result.IsValid)
                {
                    return _handleErrorValidator.HandleError(result);
                }
            }
            return await _importExportInventoryHistoryService.ImportMaterial(ImportExportInventoryRequests);
        }

        [HttpPost("fulfill-material")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> FulfillMatertial(List<ImportExportInventoryRequest> ImportExportInventoryRequests)
        {
            foreach (var request in ImportExportInventoryRequests)
            {
                var result = await _validator.ValidateAsync(request);
                if (!result.IsValid)
                {
                    return _handleErrorValidator.HandleError(result);
                }
            }
            return await _importExportInventoryHistoryService.FulfillSingleMatertial(ImportExportInventoryRequests);
        }

        [HttpPost("import-material-with-excel")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> ImportMaterialWithExcel(IFormFile file)
        {
            return await _importExportInventoryHistoryService.ImportMaterialWithExcel(file);
        }

       
    }
}
