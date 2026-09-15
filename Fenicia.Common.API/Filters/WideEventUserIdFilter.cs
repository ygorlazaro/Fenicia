using Microsoft.AspNetCore.Mvc.Filters;

namespace Fenicia.Common.API.Filters;

public class WideEventUserIdFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("wide", out var wideObj) && wideObj is WideEventContext wide)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                wide.UserId = ClaimReader.UserId(user).ToString();
            }
        }

        await next();
    }
}
