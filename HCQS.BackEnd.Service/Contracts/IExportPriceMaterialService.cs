using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IExportPriceMaterialService
    {
        public Task<AppActionResult> GetExportPriceMaterialById(Guid id);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> GetLatestPrice(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateExportPriceMaterial(ExportPriceMaterialRequest ExportPriceMaterialRequest);

        public Task<IActionResult> UploadExportPriceMaterialWithExcelFile(IFormFile file);

        public Task<AppActionResult> UpdateExportPriceMaterial(ExportPriceMaterialRequest ExportPriceMaterialRequest);

        public Task<AppActionResult> DeleteExportPriceMaterialById(Guid id);
        public Task<IActionResult> GetExportPriceMaterialTemplate();
    }
}