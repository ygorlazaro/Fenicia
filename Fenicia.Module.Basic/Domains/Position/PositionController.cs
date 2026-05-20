using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Position;

/// <summary>
///     Controller for managing employee positions within a company.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class PositionController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves all positions with pagination.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of positions.</returns>
    /// <response code="200">Positions retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllPositionResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllPositionResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var positions = await sender.Send(new GetAllPositionQuery(page, perPage), ct);

            return Ok(positions);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a position by its unique identifier.
    /// </summary>
    /// <param name="id">Position's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Position details or 404 if not found.</returns>
    /// <response code="200">Position found.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Position not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPositionByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPositionByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var position = await sender.Send(new GetPositionByIdQuery(id), ct);

            return position is null ? NotFound() : Ok(position);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves all employees belonging to a specific position.
    /// </summary>
    /// <param name="id">Position's unique identifier.</param>
    /// <param name="query">Pagination query parameters.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of employees in the position.</returns>
    /// <response code="200">Employees retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}/employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetEmployeesByPositionIdResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetEmployeesByPositionIdResponse>>>> GetEmployeesByPositionIdAsync([FromRoute] Guid id, [FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await sender.Send(new GetEmployeesByPositionIdQuery(id, query.Page, query.PerPage), ct);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new position.
    /// </summary>
    /// <param name="command">Position creation command.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created position.</returns>
    /// <response code="201">Position created successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddPositionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddPositionResponse>> PostAsync([FromBody] AddPositionCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var position = await sender.Send(command, ct);

            return new CreatedResult(string.Empty, position);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing position.
    /// </summary>
    /// <param name="command">Position update command.</param>
    /// <param name="id">Position's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated position or 404 if not found.</returns>
    /// <response code="200">Position updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Position not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePositionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdatePositionResponse>> PatchAsync([FromBody] UpdatePositionCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var position = await sender.Send(command with { Id = id }, ct);

            return position is null ? NotFound() : Ok(position);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Deletes a position (soft delete).
    /// </summary>
    /// <param name="id">Position's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Position deleted successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await sender.Send(new DeletePositionCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
