using System.Net.Mime;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Configuration;

/// <summary>
/// Controller responsible for managing user configurations for companies. Provides endpoints to retrieve and upsert configurations.
/// </summary>
/// <param name="configurationService">The configuration service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ConfigurationController(IConfigurationService configurationService) : ControllerBase
{
    /// <summary>
    ///    Retrieves all configurations for the authenticated user within a specified company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of configuration responses.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ConfigurationResponse>>> GetAsync(
        [FromQuery] Guid companyId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

            var result = await configurationService.GetAllAsync(userId, companyId, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///   Upserts a configuration for the authenticated user within a specified company. If the configuration already exists, it will be updated; otherwise, a new configuration will be created.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="request">The configuration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> PatchAsync(
        [FromQuery] Guid companyId,
        [FromBody] ConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            request.UserId = userId;

            await configurationService.UpsertAsync(request, companyId, cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
