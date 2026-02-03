using System.Net.Mime;

using Fenicia.Common.Api;
using Fenicia.Common.Database.Requests;

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
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Forgot Password Request";
        wide.UserId = request.Email;

        var response = await forgotPasswordService.SaveForgotPasswordAsync(request, cancellationToken);

        return response.Data is null ? StatusCode((int)response.Status, response.Message) : Ok(response);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Reset Password Request";
        wide.UserId = request.Email;

        var response = await forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

        return response.Data switch
        {
            null => StatusCode((int)response.Status, response.Message),
            _ => Ok(response)
        };
    }
}
