using CodeCart.Core.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace CodeCart.API.Helpers;

[AttributeUsage(AttributeTargets.All)]
public class CacheAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices
            .GetService<IResponseCacheService>();

        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

        var cachedResponse =await cacheService?.GetCachedResponseAsync(cacheKey);
        
        if(!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult()
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode=200
            };

            context.Result = contentResult;
            return;
        }

        var excutedContext = await next();

        if(excutedContext.Result is OkObjectResult okObjectResult)
        {
            if(okObjectResult.Value is not null)
            {
                await cacheService?.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
            }
        }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder(); 

        keyBuilder.Append($"{request.Path}");

        foreach (var (key,value) in request.Query.OrderBy(x=>x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
