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
        private readonly IValidator<FilterConstructionConfigRequest> _deleteValidator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IConstructionConfigValueService _constructionConfigValueService;

        public ConstructionConfigController(IValidator<ConstructionConfigRequest> createValidator, 
                                            IValidator<SearchConstructionConfigRequest> searchValidator, 
                                            IValidator<FilterConstructionConfigRequest> deleteValidator, 
                                            HandleErrorValidator handleErrorValidator, 
                                            IConstructionConfigValueService constructionConfigValueService)
        {
            _createValidator = createValidator;
            _searchValidator = searchValidator;
            _deleteValidator = deleteValidator;
            _handleErrorValidator = handleErrorValidator;
            _constructionConfigValueService = constructionConfigValueService;
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
            return await _constructionConfigValueService.CreateConstructionConfig(request);
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
            return await _constructionConfigValueService.UpdateConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-construction-config")]
        public async Task<AppActionResult> DeleteConstructionConfig(FilterConstructionConfigRequest request)
        {
            var result = await _deleteValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigValueService.DeleteConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-construction-config/{id}")]
        public async Task<AppActionResult> DeleteConstructionConfigById(Guid Id)
        {
            return await _constructionConfigValueService.DeleteConstructionConfigById(Id);
        }

        [HttpDelete("delete-all-construction-config")]
        public async Task<AppActionResult> DeleteAllConstructionConfig()
        {
            return await _constructionConfigValueService.DeleteAllConstructionConfig();
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
            return await _constructionConfigValueService.GetConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-all")]
        public async Task<AppActionResult> GetAll()
        {
            return await _constructionConfigValueService.GetAll();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("search-construction-config")]
        public async Task<AppActionResult> SearchConstructionConfig(FilterConstructionConfigRequest request)
        {
            return await _constructionConfigValueService.SearchConstructionConfig(request);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-max-config")]
        public async Task<AppActionResult> GetMaxConfig()
        {
            return await _constructionConfigValueService.GetMaxConfig();
        }
    }
}