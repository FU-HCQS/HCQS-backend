using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HCQS.BackEnd.API.Attributes
{
    public class RemoveCacheAtrribute : Attribute, IAsyncActionFilter
    {
        private readonly string pathEndPoint;

        public RemoveCacheAtrribute(string pathEndPoint)
        {
            this.pathEndPoint = $"/{pathEndPoint}/";
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
            if (!cacheConfiguration.Enabled)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var result = await next();
            if (result.Result is AppActionResult okObjectResult && okObjectResult.IsSuccess)
            {
                await cacheService.RemoveCacheResponseAsync(pathEndPoint);
            }
        }
    }
}