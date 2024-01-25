using FluentValidation;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.DAL.Util;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IValidator<BlogRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private IBlogService _blogService;

        public BlogController(IValidator<BlogRequest> validator, IBlogService blogService, HandleErrorValidator handleErrorValidator)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _blogService = blogService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        [HttpPost("create-blog")]
        [Consumes("multipart/form-data")]
        public async Task<AppActionResult> CreateBlog([FromForm] BlogRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _blogService.CreateBlog(request);
        }

        [HttpPut("update-blog")]
        [Consumes("multipart/form-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> UpdateBlog([FromForm] BlogRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _blogService.UpdateBlog(request);
        }

        [HttpDelete("delete-blog-by-id/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Permission.STAFF)]
        public async Task<AppActionResult> DeleteBlogById(Guid Id)
        {
            return await _blogService.DeleteBlogById(Id);
        }

        [HttpGet("get-blog-by-id/{Id}")]
        public async Task<AppActionResult> GetBlogById(Guid Id)
        {
            return await _blogService.GetBlogById(Id);
        }

        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _blogService.GetAll(pageIndex, pageSize, sortInfos);
        }
    }
}