using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.Api.Controllers;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Returns a 403 Forbidden response with custom problem details for business permission errors.
    /// Replaces Forbid(ex.Message) to avoid invalid scheme exceptions.
    /// </summary>
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
