using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IProgressConstructionMaterialService
    {
        public Task<AppActionResult> GetProgressConstructionMaterialById(Guid id);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);
        public Task<AppActionResult> GetAllByQuotationId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);
        public Task<AppActionResult> GetAllByQuotationDetailId(Guid Id, int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateProgressConstructionMaterial(List<ProgressConstructionMaterialRequest> ProgressConstructionMaterialRequest);

        public Task<AppActionResult> UpdateProgressConstructionMaterial(Guid Id, ProgressConstructionMaterialRequest ProgressConstructionMaterialRequest);

        public Task<AppActionResult> DeleteProgressConstructionMaterialById(Guid id);
    }
}
