using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
///     Controller responsible for handling forgot password-related HTTP endpoints.
///     Provides endpoints to initiate password reset and complete password reset.
/// </summary>
/// <remarks>
///     These endpoints are publicly accessible (AllowAnonymous) to allow users to recover their accounts.
/// </remarks>
[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(AddForgotPasswordHandler addForgotPasswordHandler, ResetPasswordHandler resetPasswordHandler) : ControllerBase
{
    /// <summary>
    ///     Initiates the forgot password process by generating a reset code for the user.
    /// </summary>
    /// <param name="reset">The command containing the user's email address.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result on successful initiation.</returns>
    /// <response code="201">Password reset code created successfully.</response>
    /// <response code="400">Invalid request or user not found.</response>
    /// <exception cref="ItemNotExistsException">User not found with the given email.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] AddForgotPasswordCommand reset, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = reset.Email;

            await addForgotPasswordHandler.Handle(reset, ct);

            return Created();
        }
        catch (ItemNotExistsException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    ///     Completes the password reset process using the provided code.
    /// </summary>
    /// <param name="request">The command containing email, new password, and reset code.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created result on successful password reset.</returns>
    /// <response code="201">Password reset successfully.</response>
    /// <response code="400">Invalid code, expired code, or user not found.</response>
    /// <exception cref="ItemNotExistsException">User not found with the given email.</exception>
    /// <exception cref="InvalidDataException">The code is invalid, expired, or already used.</exception>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            await resetPasswordHandler.Handle(request, ct);

            return Created();
        }
        catch (ItemNotExistsException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}