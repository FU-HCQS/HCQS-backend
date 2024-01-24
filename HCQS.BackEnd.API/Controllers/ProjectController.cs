using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create-project-by-user")]
        public async Task<AppActionResult> CreateProjectByUser([FromForm]ProjectDto projectDto)
        {
            return await _projectService.CreateProjectByUser(projectDto);
        }
        [HttpPut("config-project")]
        public async Task<AppActionResult> ConfigProject(ConfigProjectRequest projectDto)
        {
            return await _projectService.ConfigProject(projectDto);
        }
        [HttpGet("get-all-project")]
        public async Task<AppActionResult> GetAllProject()
        {
            return await _projectService.GetAllProject();
        }
        [HttpGet("get-project-by-id/{id}")]
        public async Task<AppActionResult> GetProjectById(Guid id)
        {
            return await _projectService.GetProjectById(id);
        }
    }
}
