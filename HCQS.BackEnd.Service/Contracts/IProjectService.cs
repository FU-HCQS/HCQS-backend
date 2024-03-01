using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IProjectService
    {
        public Task<AppActionResult> CreateProjectByUser(ProjectDto projectDto);

        public Task<AppActionResult> ConfigProject(ConfigProjectRequest project);

        public Task<AppActionResult> GetAllProject(Project.ProjectStatus status);

        public Task<AppActionResult> GetAllProjectByAccountId(string accountId, Project.ProjectStatus status);

        public Task<AppActionResult> GetProjectById(Guid id);

        public Task<AppActionResult> GetProjectByIdForCustomer(Guid id);
    }
}