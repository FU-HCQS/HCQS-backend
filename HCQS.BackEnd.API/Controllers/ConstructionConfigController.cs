using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Dto;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;
using Org.BouncyCastle.Asn1.Ocsp;
using HCQS.BackEnd.DAL.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("construction-config")]
    [ApiController]
    public class ConstructionConfigController : Controller
    {
        private readonly IValidator<ConstructionConfigRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IConstructionConfigService _constructionConfigService;

        public ConstructionConfigController(IValidator<ConstructionConfigRequest> validator, HandleErrorValidator handleErrorValidator, IConstructionConfigService constructionConfigService)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _constructionConfigService = constructionConfigService;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-construction-config")]
        public async Task<AppActionResult> CreateConstructionConfig(ConstructionConfigRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.CreateConstructionConfig(request);
        }



        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPut("update-construction-config")]
        public async Task<AppActionResult> UpdateConstructionConfig(ConstructionConfigRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _constructionConfigService.UpdateConstructionConfig(request);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpDelete("delete-construction-config-by-id/{Id}")]
        public async Task<AppActionResult> DeleteConstructionConfig(Guid Id){

            return await _constructionConfigService.DeleteConstructionConfig(Id);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-construction-config-of-project/{projectId}")]
        public async Task<AppActionResult> GetConstructionConfig(Guid projectId)
        {
            return await _constructionConfigService.GetConstructionConfig(projectId);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpGet("get-all")]
        public async Task<AppActionResult> GetAll(Guid projectId)
        {
            return await _constructionConfigService.GetAll();
        }
    }
}
