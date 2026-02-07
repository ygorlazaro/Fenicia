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
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = request.Email;

        var response = await forgotPasswordService.SaveForgotPasswordAsync(request, ct);

        return Ok(response);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = request.Email;

        var response = await forgotPasswordService.ResetPasswordAsync(request, ct);

        return Ok(response);
    }
}
