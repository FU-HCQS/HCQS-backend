using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IProjectService
    {
        public Task<AppActionResult> CreateProjectByUser(ProjectDto projectDto);

        public Task<AppActionResult> ConfigProject(ConfigProjectRequest project);

        public Task<AppActionResult> GetAllProject();

        public Task<AppActionResult> GetAllProjectByAccountId(string accountId);

        public Task<AppActionResult> GetProjectById(Guid id);

        public Task<AppActionResult> GetProjectByIdForCustomer(Guid id);
    }
}