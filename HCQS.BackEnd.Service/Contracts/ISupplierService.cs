using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface ISupplierService
    {
        public Task<AppActionResult> GetSupplierById(Guid id);
        Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);
        public Task<AppActionResult> CreateSupplier(SupplierRequest supplierRequest);
        public Task<AppActionResult> UpdateSupplier(SupplierRequest supplierRequest);
        public Task<AppActionResult> DeleteSupplierById(Guid id);

        public Task<AppActionResult> ImportSupplierWithExcelFile(IFormFile file);
    }
}
