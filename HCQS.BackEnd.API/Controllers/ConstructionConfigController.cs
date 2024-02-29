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
    [Route("construction-config")]
    [ApiController]
    public class ConstructionConfigController : Controller
    {
        private readonly IValidator<ConstructionConfigRequest> _createValidator;
        private readonly IValidator<SearchConstructionConfigRequest> _searchValidator;
        private readonly IValidator<DeleteConstructionConfigRequest> _deleteValidator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IConstructionConfigService _constructionConfigService;

        public ConstructionConfigController(IValidator<ConstructionConfigRequest> createValidator, IValidator<SearchConstructionConfigRequest> searchValidator, IValidator<DeleteConstructionConfigRequest> deleteValidator, HandleErrorValidator handleErrorValidator, IConstructionConfigService constructionConfigService)
        {
            _createValidator = createValidator;
            _searchValidator = searchValidator;
            _deleteValidator = deleteValidator;
            _handleErrorValidator = handleErrorValidator;
            _constructionConfigService = constructionConfigService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-construction-config")]
        public async Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request)
        {
            var result = await _createValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.CreateConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPut("update-construction-config")]
        public async Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request)
        {
            var result = await _createValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.UpdateConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-construction-config-by-name-and-value")]
        public async Task<AppActionResult> CreateConstructionConfig(string name, float value)
        {
            return await _constructionConfigService.CreateConstructionConfig(name, value);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-construction-config")]
        public async Task<AppActionResult> DeleteConstructionConfig(DeleteConstructionConfigRequest request)
        {
            var result = await _deleteValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.DeleteConstructionConfig(request);
        }

        [HttpDelete("delete-all-construction-config")]
        public async Task<AppActionResult> DeleteAllConstructionConfig()
        {
            return await _constructionConfigService.DeleteAllConstructionConfig();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("get-construction-config")]
        public async Task<AppActionResult> GetConstructionConfig(SearchConstructionConfigRequest request)
        {
            var result = await _searchValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.GetConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-all")]
        public async Task<AppActionResult> GetAll()
        {
            return await _constructionConfigService.GetAll();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("search-construction-config/{keyword}")]
        public async Task<AppActionResult> SearchConstructionConfig(string keyword)
        {
            return await _constructionConfigService.SearchConstructionConfig(keyword);
        }
    }
}