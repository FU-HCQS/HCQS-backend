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

namespace HCQS.BackEnd.API.Controllers
{
    [Route("supplier-price-quotation")]
    [ApiController]
    public class SupplierPriceQuotationController : Controller
    {
        private readonly HandleErrorValidator _handleErrorValidator;
        private ISupplierPriceQuotationService _supplierPriceQuotationService;

        public SupplierPriceQuotationController(ISupplierPriceQuotationService supplierPriceQuotationService, HandleErrorValidator handleErrorValidator)
        {
            _handleErrorValidator = handleErrorValidator;
            _supplierPriceQuotationService = supplierPriceQuotationService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("Upload-supplier-quotation-with-excel-file")]
        public async Task<AppActionResult> UploadSupplierQuotationWithExcelFile(IFormFile file)
        {
            return await _supplierPriceQuotationService.UploadSupplierQuotationWithExcelFile(file);
        }


        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceQuotationService.GetAll(pageIndex, pageSize, sortInfos);
        }

        [HttpPost("get-all-by-month")]
        public async Task<AppActionResult> GetAll(int month, int year, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceQuotationService.GetQuotationByMonth(month, year, pageIndex, pageSize, sortInfos);
        }
    }
}
