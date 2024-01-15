using FluentValidation;
using HCQS.BackEnd.Common.Dto.BaseRequest;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.Common.Validator;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("news")]
    [ApiController]
    public class NewsController : Controller
    {
        private readonly IValidator<NewsRequest> _validator;
        private readonly HandleErrorValidator _handleErrorValidator;
        private INewsService _newsService;
        public NewsController(IValidator<NewsRequest> validator, HandleErrorValidator handleErrorValidator, INewsService service)
        {
            _validator = validator;
            _handleErrorValidator = handleErrorValidator;
            _newsService = service;
        }

        [HttpPost("create-news")]
        public async Task<AppActionResult> CreateNews([FromForm] NewsRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _newsService.CreateNews(request);

        }

        [HttpPut("update-news")]
        public async Task<AppActionResult> UpdateBlog([FromForm] NewsRequest request)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return _handleErrorValidator.HandleError(result);
            }
            return await _newsService.UpdateNews(request);

        }
        [HttpDelete("delete-news-by-id/{Id}")]
        public async Task<AppActionResult> DeleteNewsById(Guid Id)
        {
            return await _newsService.DeleteNewsById(Id);

        }
        [HttpGet("get-news-by-id/{Id}")]
        public async Task<AppActionResult> GetNewsById(Guid Id)
        {
            return await _newsService.GetNewsById(Id);

        }
        [HttpPost("get-all")]
        public async Task<AppActionResult> GetAll(int pageIndex, int pageSize, IList<SortInfo> sortInfos)
        {
            return await _newsService.GetAll(pageIndex, pageSize, sortInfos);
        }
    }
}
