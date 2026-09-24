using System.Net.Mime;
using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

/// <summary>
/// Controller for managing user registration operations.
/// </summary>
[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(IRegisterService registerService) : ControllerBase
{
    /// <summary>
    /// Creates a new user with their initial company.
    /// </summary>
    /// <param name="request">User data (email, password, name, company)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user and company data</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Email already exists, company already exists, or invalid data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<RegisterResponse>> CreateNewUserAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userResponse = await registerService.CreateAsync(request, cancellationToken);

            return Created(string.Empty, userResponse);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
