using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Auth.Domains.ForgotPassword.Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(ILogger<ForgotPasswordController> logger, IForgotPasswordService forgotPasswordService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Request forgot password");

        var response = await forgotPasswordService.SaveForgotPasswordAsync(request, cancellationToken);

        return response.Data switch
        {
            null => StatusCode((int)response.Status, response.Message),
            _ => Ok(response)
        };
    }

    [HttpPost("reset")]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reset password request");

        var response = await forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

        return response.Data switch
        {
            null => StatusCode((int)response.Status, response.Message),
            _ => Ok(response)
        };
    }
}
