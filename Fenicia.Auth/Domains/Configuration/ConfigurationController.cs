using System.Net.Mime;

using Fenicia.Auth.Domains.Configuration.Commands;

using Fenicia.Auth.Domains.Configuration.Queries;
using Fenicia.Auth.Domains.Configuration.Responses;
using Fenicia.Common.API;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Configuration;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ConfigurationController(ISender sender) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetConfigurationResponse>>> GetAsync([FromQuery] Guid companyId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var query = new GetConfigurationQuery(userId, companyId);
            var result = await sender.Send(query, ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> PatchAsync([FromRoute] Guid id, [FromBody] UpsertConfigurationCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var command = request with { UserId = userId, Id = id };
            await sender.Send(command, ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
