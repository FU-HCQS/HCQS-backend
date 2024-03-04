using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IConstructionConfigValueService
    {
        Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request);

        Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request);

        Task<AppActionResult> DeleteConstructionConfig(FilterConstructionConfigRequest request);
        Task<AppActionResult> DeleteConstructionConfigById(Guid Id);

        Task<AppActionResult> DeleteAllConstructionConfig();

        Task<AppActionResult> GetConstructionConfig(SearchConstructionConfigRequest request);
        Task<AppActionResult> GetMaxConfig();

        Task<AppActionResult> SearchConstructionConfig(FilterConstructionConfigRequest request);

        Task<AppActionResult> GetAll();
    }
}
