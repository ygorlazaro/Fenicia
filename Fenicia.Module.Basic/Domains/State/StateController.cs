using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.State.Handlers;
using Fenicia.Module.Basic.Domains.State.Responses;

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
public class StateController(GetAllStateHandler getAllStateHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a list of all Brazilian states.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of all states.</returns>
    /// <response code="200">Returns the list of states successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllStateResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllStateResponse>>> GetAllAsync(WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var states = await getAllStateHandler.Handle(ct);

        return Ok(states);
    }
}