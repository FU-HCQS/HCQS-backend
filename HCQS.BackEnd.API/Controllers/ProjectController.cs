using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("project")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private IProjectService _projectService;
        private readonly IValidator<ProjectDto> _validator;
        private readonly IValidator<ConfigProjectRequest> _validatorConfig;

        private readonly HandleErrorValidator _handleErrorValidator;

        public ProjectController(IProjectService projectService, IValidator<ProjectDto> validator, IValidator<ConfigProjectRequest> validatorConfig, HandleErrorValidator handleErrorValidator)
        {
            _projectService = projectService;
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _validatorConfig = validatorConfig;
        }

        [HttpPost("create-project-by-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.CUSTOMER)]
        public async Task<AppActionResult> CreateProjectByUser([FromForm] ProjectDto projectDto)
        {
            var result = await _validator.ValidateAsync(projectDto);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _projectService.CreateProjectByUser(projectDto);
        }

        [HttpPut("config-project")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> ConfigProject(ConfigProjectRequest projectDto)
        {
            var result = await _validatorConfig.ValidateAsync(projectDto);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _projectService.ConfigProject(projectDto);
        }

        [HttpGet("get-all-project")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.MANAGEMENT)]
        public async Task<AppActionResult> GetAllProject()
        {
            return await _projectService.GetAllProject();
        }

        [HttpGet("get-all-project-by-accountId/{accountId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.CUSTOMER)]
        public async Task<AppActionResult> GetAllProjectByAccountId(string accountId)
        {
            return await _projectService.GetAllProjectByAccountId(accountId);
        }

        [HttpGet("get-project-by-id/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.MANAGEMENT)]

        public async Task<AppActionResult> GetProjectById(Guid id)
        {
            return await _projectService.GetProjectById(id);
        }

        [HttpGet("get-project-by-id-for-customer/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.CUSTOMER)]

        public async Task<AppActionResult> GetProjectByIdForCustomer(Guid id)
        {
            return await _projectService.GetProjectByIdForCustomer(id);
        }
    }
}