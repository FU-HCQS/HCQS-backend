using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISupplierService
    {
        public Task<AppActionResult> GetSupplierById(Guid id);

        public Task<AppActionResult> GetSupplierByName(String name);

        Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateSupplier(SupplierRequest supplierRequest);

        public Task<AppActionResult> UpdateSupplier(SupplierRequest supplierRequest);

        public Task<AppActionResult> DeleteSupplierById(Guid id);

        public Task<IActionResult> ImportSupplierWithExcelFile(IFormFile file);
    }
}