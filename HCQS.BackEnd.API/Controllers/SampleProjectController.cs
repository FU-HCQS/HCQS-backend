using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Common;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("sample-project")]
    [ApiController]
    public class SampleProjectController : ControllerBase
    {
        private readonly IValidator<SampleProjectRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private ISampleProjectService _sampleProjectService;

        public SampleProjectController(IValidator<SampleProjectRequest> validator, ISampleProjectService sampleProjectService, HandleErrorValidator handleErrorValidator)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _sampleProjectService = sampleProjectService;
        }

        [HttpPost("create-sample-project")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> CreateSampleProject([FromForm] SampleProjectRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _sampleProjectService.CreateSampleProject(request);
        }

        [HttpPut("update-sample-project")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateSampleProject([FromForm] SampleProjectRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _sampleProjectService.UpdateSampleProject(request);
        }

        [HttpDelete("delete-sample-project-by-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteSampleProjectById(Guid Id)
        {
            return await _sampleProjectService.DeleteSampleProjectById(Id);
        }

        [HttpGet("get-sample-project-by-id/{Id}")]
        public async Task<AppActionResult> GetSampleProjectById(Guid Id)
        {
            return await _sampleProjectService.GetSampleProjectById(Id);
        }

        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _sampleProjectService.GetAll(pageIndex, pageSize, sortInfos);
        }
    }
}