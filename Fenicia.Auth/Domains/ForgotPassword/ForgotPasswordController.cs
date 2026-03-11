using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(
    AddForgotPasswordHandler addForgotPasswordHandler,
    ResetPasswordHandler resetPasswordHandler
    ) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task ForgotPassword(
        [FromBody] AddForgotPasswordCommand reset,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = reset.Email;

        await addForgotPasswordHandler.Handle(reset, ct);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand request,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.Email;

        await resetPasswordHandler.Handle(request, ct);

        return Ok();
    }
}
