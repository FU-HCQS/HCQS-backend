using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("supplier-price-detail")]
    [ApiController]
    public class SupplierPriceDetailController : Controller
    {
        public ISupplierPriceDetailService _supplierPriceDetailService;

        public SupplierPriceDetailController(ISupplierPriceDetailService supplierPriceDetailService)
        {
            _supplierPriceDetailService = supplierPriceDetailService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-quotation-price-by-material-id/{Id}")]
        public async Task<AppActionResult> GetQuotationPriceByMaterialId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceDetailService.GetQuotationPricesByMaterialId(Id, pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-quotation-price-by-supplier-id/{Id}")]
        public async Task<AppActionResult> GetQuotationPriceBySupplierId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceDetailService.GetQuotationPricesBySupplierId(Id, pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-quotation-price-by-material-name/{name}")]
        public async Task<AppActionResult> GetQuotationPriceByMaterialId(String name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceDetailService.GetQuotationPricesByMaterialName(name, pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-quotation-price-by-supplier-name/{name}")]
        public async Task<AppActionResult> GetQuotationPriceBySupplierId(String name, int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceDetailService.GetQuotationPricesBySupplierName(name, pageIndex, pageSize, sortInfos);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _supplierPriceDetailService.GetAll(pageIndex, pageSize, sortInfos);
        }
    }
}
