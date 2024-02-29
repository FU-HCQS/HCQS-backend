using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IConstructionConfigService
    {
        Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request);
        Task<AppActionResult> CreateConstructionConfig(string name, float value);
        Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request);
        Task<AppActionResult> DeleteConstructionConfig(Guid Id);
        Task<AppActionResult> GetConstructionConfig(Guid projectId);
        Task<AppActionResult> GetAll();
    }
}
