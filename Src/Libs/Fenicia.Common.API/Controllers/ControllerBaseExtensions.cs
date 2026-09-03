using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API.Controllers;

public static class ControllerBaseExtensions
{
    public static ObjectResult ForbidWithMessage(this ControllerBase controller, string message)
    {
        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            title = "Forbidden",
            status = 403,
            detail = message
        };
        return controller.StatusCode(403, problem);
    }
}