using System.Net.Mime;

using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Auth.Domains.Configuration.Handlers;
using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Auth.Domains.Configuration.Responses;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Configuration;

/// <summary>
/// Controller responsible for handling configuration-related HTTP endpoints.
/// Provides endpoints to retrieve and update user/company configurations such as language and timezone.
/// </summary>
/// <remarks>
/// All endpoints require authentication. Configurations are scoped by user and optionally by company.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ConfigurationController(
    GetConfigurationHandler getConfigurationHandler,
    UpsertConfigurationHandler upsertConfigurationHandler) : ControllerBase
{
    /// <summary>
    /// Retrieves configurations for the authenticated user, optionally filtered by company.
    /// </summary>
    /// <param name="companyId">Optional company ID to filter configurations.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of configuration responses.</returns>
    /// <response code="200">Returns the list of configurations successfully.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetConfigurationResponse>>> GetAsync(
        [FromQuery] Guid? companyId,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var query = new GetConfigurationQuery(userId, companyId);
        var result = await getConfigurationHandler.Handle(query, ct);

        return Ok(result);
    }

    /// <summary>
    /// Creates or updates a configuration entry.
    /// Uses upsert pattern: creates new if doesn't exist, updates existing otherwise.
    /// </summary>
    /// <param name="id">The configuration ID (used for routing).</param>
    /// <param name="request">The upsert command containing configuration details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful upsert.</returns>
    /// <response code="204">Configuration created or updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] UpsertConfigurationCommand request,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var command = request with { UserId = userId, Id = id };
        await upsertConfigurationHandler.Handle(command, ct);

        return NoContent();
    }
}
