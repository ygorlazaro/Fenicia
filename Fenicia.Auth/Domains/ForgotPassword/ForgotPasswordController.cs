using System.Net.Mime;
using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.ForgotPassword;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
/// Controller responsible for handling forgot password operations, including initiating password reset requests and resetting user passwords.
/// </summary>
/// <param name="forgotPasswordService"></param>
[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ForgotPasswordController(IForgotPasswordService forgotPasswordService) : ControllerBase
{
    /// <summary>
    ///   Initiates a forgot password request for a user. This endpoint generates a unique code and sends it to the user's registered email address, allowing them to reset their password.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PostAsync(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            request.IpAddress = ipAddress;
            request.UserAgent = userAgent;
            request.UserId = userId;

            await forgotPasswordService.AddAsync(request, cancellationToken);

            return Created();
        }
        catch (ForbiddenException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    ///   Resets the password for a user based on the provided reset password request. This endpoint validates the reset code and updates the user's password if the code is valid and has not expired.
    /// </summary>
    /// <param name="request">The reset password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ForbiddenException"></exception>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PatchAsync(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await forgotPasswordService.ResetAsync(request, cancellationToken);

            return NoContent();
        }
        catch (ForbiddenException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
