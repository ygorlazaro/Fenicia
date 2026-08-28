using System.Net.Mime;
using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(RegisterService registerService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<RegisterResponse>> CreateNewUserAsync(RegisterCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await registerService.CreateAsync(request, ct);

            return Created(string.Empty, userResponse);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
