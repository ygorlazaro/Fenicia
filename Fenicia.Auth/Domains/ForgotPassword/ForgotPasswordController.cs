using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.ResetPassword;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task ForgotPassword(
        [FromBody] AddForgotPasswordCommand reset,
        [FromServices] AddForgotPasswordHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = reset.Email;

        await handler.Handle(reset, ct);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand request,
        [FromServices] ResetPasswordHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.Email;

        await handler.Handle(request, ct);

        return Ok();
    }
}