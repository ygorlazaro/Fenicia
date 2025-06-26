using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Auth.Domains.ForgotPassword.Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
/// Handles password recovery and reset operations for users
/// </summary>
/// <remarks>
/// This controller provides endpoints for initiating password recovery and completing password reset
/// </remarks>
[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(ILogger<ForgotPasswordController> logger, IForgotPasswordService forgotPasswordService) : ControllerBase
{
    /// <summary>
    /// Initiates the password recovery process for a user
    /// </summary>
    /// <param name="request">The password recovery request details</param>
    /// <param name="cancellationToken">Cancellation token for handling cancellation requests</param>
    /// <returns>Action result indicating the success or failure of the operation</returns>
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

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password recovery process");
            throw;
        }
    }

    /// <summary>
    /// Resets the user's password using the provided reset token
    /// </summary>
    /// <param name="request">The password reset request containing the new password and reset token</param>
    /// <param name="cancellationToken">Cancellation token for handling cancellation requests</param>
    /// <returns>Action result indicating the success or failure of the password reset</returns>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody]ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting password reset process");

            var response = await forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

            logger.LogInformation("Password reset process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password reset process");
            throw;
        }
    }
}
