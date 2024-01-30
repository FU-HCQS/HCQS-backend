using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IImportExportInventoryHistoryService
    {
        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetAllImport(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetAllExport(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> ImportMaterial(List<ImportExportInventoryRequest> ImportExportInventoryRequests);

        public Task<AppActionResult> UpdateInventory(Guid Id, ImportExportInventoryRequest ImportExportInventoryRequests);

        public Task<AppActionResult> FulfillSingleMatertial(List<ImportExportInventoryRequest> ImportExportInventoryRequests);

        public Task<IActionResult> ImportMaterialWithExcel(IFormFile file);

        public Task<AppActionResult> FulfillMaterialWithExcel(IFormFile file);
        public Task<IActionResult> GetImportInventoryTempate();
    }
}