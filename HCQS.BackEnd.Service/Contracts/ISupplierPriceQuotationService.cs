using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISupplierPriceQuotationService
    {
        public Task<IActionResult> UploadSupplierQuotationWithExcelFile(IFormFile file);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetQuotationByMonth(int month, int year, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateSupplierPriceQuotation(SupplierPriceQuotationRequest supplierPriceQuotationRequest);

        public Task<AppActionResult> UpdateSupplierPriceQuotation(SupplierPriceQuotationRequest supplierPriceQuotationRequest);

        public Task<AppActionResult> DeleteSupplierPriceQuotationById(Guid id);

        public Task<IActionResult> GetPriceQuotationTemplate();
    }
}