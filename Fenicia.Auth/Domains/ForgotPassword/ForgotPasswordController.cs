namespace Fenicia.Auth.Domains.ForgotPassword;

using System.Net.Mime;

using Data;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[AllowAnonymous]
[Route(template: "[controller]")]
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
            logger.LogInformation(message: "Starting password recovery process for user request");

            var response = await forgotPasswordService.SaveForgotPasswordAsync(request, cancellationToken);

            logger.LogInformation(message: "Password recovery process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error during password recovery process");
            throw;
        }
    }

    [HttpPost(template: "reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting password reset process");

            var response = await forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

            logger.LogInformation(message: "Password reset process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error during password reset process");
            throw;
        }
    }
}
