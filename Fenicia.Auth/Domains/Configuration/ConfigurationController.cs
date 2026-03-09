using System.Net.Mime;

using Fenicia.Auth.Domains.Configuration.GetConfiguration;
using Fenicia.Auth.Domains.Configuration.UpsertConfiguration;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Configuration;

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
    /// Get all configurations for the current user
    /// </summary>
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
    /// Create or update a configuration (upsert)
    /// </summary>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> PatchAsync(
        [FromBody] UpsertConfigurationCommand request,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var command = request with { UserId = userId };
        await upsertConfigurationHandler.Handle(command, ct);

        return NoContent();
    }
}
