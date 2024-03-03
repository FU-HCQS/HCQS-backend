using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IConstructionConfigService
    {
        Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request);

        Task<AppActionResult> CreateConstructionConfig(string name, float value);

        Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request);

        Task<AppActionResult> DeleteConstructionConfig(DeleteConstructionConfigRequest request);

        Task<AppActionResult> DeleteAllConstructionConfig();

        Task<AppActionResult> GetConstructionConfig(SearchConstructionConfigRequest request);
        Task<AppActionResult> GetMaxConfig();

        Task<AppActionResult> SearchConstructionConfig(string keyword);

        Task<AppActionResult> GetAll();
    }
}