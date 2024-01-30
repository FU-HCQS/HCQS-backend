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
    [Route("material")]
    [ApiController]
    public class MaterialController : Controller
    {
        private readonly IValidator<MaterialRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IMaterialService _materialService;

        public MaterialController(IValidator<MaterialRequest> validator, IMaterialService materialService, HandleErrorValidator handleErrorValidator)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _materialService = materialService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-material")]
        public async Task<AppActionResult> CreateMaterial(MaterialRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _materialService.CreateMaterial(request);
        }

        [HttpPut("update-material")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateMaterial(MaterialRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _materialService.UpdateMaterial(request);
        }

        [HttpPut("update-material-quantity")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateMaterialQuantity(Guid Id, int addQuantity)
        {
            return await _materialService.UpdateQuantityById(Id, addQuantity);
        }

        [HttpDelete("delete-material-by-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteMaterialById(Guid Id)
        {
            return await _materialService.DeleteMaterialById(Id);
        }

        [HttpGet("get-material-by-id/{Id}")]
        public async Task<AppActionResult> GetMaterialById(Guid Id)
        {
            return await _materialService.GetMaterialById(Id);
        }

        [HttpGet("get-material-by-name/{name}")]
        public async Task<AppActionResult> GetMaterialByName(string name)
        {
            return await _materialService.GetMaterialByName(name);
        }

        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _materialService.GetAll(pageIndex, pageSize, sortInfos);
        }
    }
}