using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.State.Queries;
using Fenicia.Module.Basic.Domains.State.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.State;

/// <summary>
///     Controller responsible for handling state-related HTTP endpoints.
///     Provides endpoint to retrieve all Brazilian states.
/// </summary>
/// <remarks>
///     All endpoints require authentication. States are used for address localization.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StateController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves a list of all Brazilian states.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of all states.</returns>
    /// <response code="200">Returns the list of states successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllStateResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllStateResponse>>> GetAllAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var states = await sender.Send(new GetAllStateQuery(), ct);

            return Ok(states);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
