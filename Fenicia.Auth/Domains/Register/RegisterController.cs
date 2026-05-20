using System.Net.Mime;

using Fenicia.Auth.Domains.Register.Command;
using Fenicia.Auth.Domains.Register.Response;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

/// <summary>
///     Controller responsible for handling user registration.
///     Provides public endpoint for new users to register along with their company.
/// </summary>
/// <remarks>
///     This is a public endpoint (no authentication required) that allows:
///     1. Creating a new user account
///     2. Creating a new company (tenant)
///     3. Assigning Admin role to the new user for their company
///     The registering user becomes the first Admin of their company,
///     allowing them to invite other users and configure settings.
///     Related documentation:
///     - See <see cref="Fenicia.Auth.Domains.User.Handlers.CreateNewUserHandler" /> for user creation details
///     - See [CompanyDomain](../auth/CompanyDomain.md) for company management
/// </remarks>
[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Registers a new user and creates their company.
    /// </summary>
    /// <param name="request">The registration request containing user info and company details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created user response with company information.</returns>
    /// <remarks>
    ///     Validates:
    ///     - Email is not already registered
    ///     - Company CNPJ is not already registered
    ///     - Admin role exists in the system
    ///     Creates:
    ///     - New user with hashed password
    ///     - New company with provided details
    ///     - User role assignment with Admin role
    /// </remarks>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request (duplicate email, duplicate CNPJ, or missing Admin role).</response>
    /// <exception cref="InvalidRequestException">Email already exists, CNPJ already exists, or Admin role not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<RegisterResponse>> CreateNewUserAsync(RegisterCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await sender.Send(request, ct);

            return Created(string.Empty, userResponse);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
