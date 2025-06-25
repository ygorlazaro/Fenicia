using System.Net.Mime;

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
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        logger.LogInformation("Request forgot password");

        var response = await forgotPasswordService.SaveForgotPasswordAsync(request);

        if (response.Data is null)
        {
            return StatusCode((int)response.Status, response.Message);
        }

        return Ok(response);
    }

    [HttpPost("reset")]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request)
    {
        logger.LogInformation("Reset password request");

        var response = await forgotPasswordService.ResetPasswordAsync(request);

        if (response.Data is null)
        {
            return StatusCode((int)response.Status, response.Message);
        }

        return Ok(response);
    }
}
