using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(IForgotPasswordService forgotPasswordService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgotPassword(
        [FromBody] ForgotPasswordReset reset,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = reset.Email;

        var response = await forgotPasswordService.SaveForgotPasswordAsync(reset, ct);

        return Ok(response);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ForgotPasswordResetRequest resetRequest,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = resetRequest.Email;

        var response = await forgotPasswordService.ResetPasswordAsync(resetRequest, ct);

        return Ok(response);
    }
}