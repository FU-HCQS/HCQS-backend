using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISupplierPriceQuotationService
    {
        public Task<AppActionResult> UploadSupplierQuotationWithExcelFile(IFormFile file);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetQuotationByMonth(int month, int year, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateSupplierPriceQuotation(SupplierPriceQuotationRequest supplierPriceQuotationRequest);

        public Task<AppActionResult> UpdateSupplierPriceQuotation(SupplierPriceQuotationRequest supplierPriceQuotationRequest);

        public Task<AppActionResult> DeleteSupplierPriceQuotationById(Guid id);
    }
}