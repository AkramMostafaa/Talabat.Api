using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Service.Contract;

namespace Route.Talabat.Api.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            // ASK CLR  For Creating Object From  "ResponseCacheService" Explixcitly 

            var cacheKey = GenrateCacheKeyFromRequest(context.HttpContext.Request);

            var response =await responseCacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode=200
                    
                };
                context.Result = result;
                return;
            }  // NOT Cached
          var executedActionContext=  await next.Invoke();    // Will Execute The Next Action Filter Or Action Itself
            if (executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }

        }

        private string GenrateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.PathBase);  // /api/products

            // pageInedx=1&pageSize=5&sort=name
            foreach (var (key,value) in request.Query.OrderBy(x=>x.Key)) 
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
