using System.Net.Mime;

using Fenicia.Common.Database.Requests;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting password recovery process for user request");

            var response = await forgotPasswordService.SaveForgotPasswordAsync(request, cancellationToken);

            logger.LogInformation("Password recovery process completed with status: {Status}", response.Status);

            return response.Data is null ? this.StatusCode((int)response.Status, response.Message) : this.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password recovery process");

            throw;
        }
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting password reset process");

            var response = await forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

            logger.LogInformation("Password reset process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => this.StatusCode((int)response.Status, response.Message),
                _ => this.Ok(response)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password reset process");

            throw;
        }
    }
}
