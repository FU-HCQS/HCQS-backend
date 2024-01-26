using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IMaterialService
    {
        public Task<AppActionResult> GetMaterialById(Guid id);

        public Task<AppActionResult> GetMaterialByName(string name);

        public Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos);

        public Task<AppActionResult> CreateMaterial(MaterialRequest MaterialRequest);

        public Task<AppActionResult> UpdateMaterial(MaterialRequest MaterialRequest);

        public Task<AppActionResult> UpdateQuantityById(Guid id, int addedQuantity);

        public Task<AppActionResult> DeleteMaterialById(Guid id);
    }
}