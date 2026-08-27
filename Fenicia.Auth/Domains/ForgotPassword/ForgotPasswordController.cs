using System.Net.Mime;

using Fenicia.Auth.Domains.ForgotPassword.Commands;

using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(ISender sender) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] AddForgotPasswordCommand reset, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = reset.Email;

            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = HttpContext?.Request?.Headers.UserAgent.ToString();

            var command = new AddForgotPasswordCommand(reset.Email, ipAddress, userAgent);

            await sender.Send(command, ct);

            return Created();
        }
        catch (ItemNotExistsException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            await sender.Send(request, ct);

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
