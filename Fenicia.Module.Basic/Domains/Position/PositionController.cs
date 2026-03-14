using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Handlers;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Position;

/// <summary>
///     Controller for managing employee positions within a company.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class PositionController(GetAllPositionHandler getAllPositionHandler, GetPositionByIdHandler getPositionByIdHandler, AddPositionHandler addPositionHandler, UpdatePositionHandler updatePositionHandler, DeletePositionHandler deletePositionHandler, GetEmployeesByPositionIdHandler getEmployeesByPositionIdHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves all positions with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllPositionResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllPositionResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var positions = await getAllPositionHandler.Handle(new GetAllPositionQuery(page, perPage), ct);

        return Ok(positions);
    }

    /// <summary>
    ///     Retrieves a position by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPositionByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetPositionByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var position = await getPositionByIdHandler.Handle(new GetPositionByIdQuery(id), ct);

        return position is null ? NotFound() : Ok(position);
    }

    /// <summary>
    ///     Retrieves all employees belonging to a specific position.
    /// </summary>
    [HttpGet("{id:guid}/employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetEmployeesByPositionIdResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetEmployeesByPositionIdResponse>>> GetEmployeesByPositionIdAsync([FromRoute] Guid id, [FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var employees = await getEmployeesByPositionIdHandler.Handle(new GetEmployeesByPositionIdQuery(id, query.Page, query.PerPage), ct);

        return Ok(employees);
    }

    /// <summary>
    ///     Creates a new position.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddPositionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddPositionResponse>> PostAsync([FromBody] AddPositionCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var position = await addPositionHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, position);
    }

    /// <summary>
    ///     Updates an existing position.
    /// </summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdatePositionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdatePositionResponse>> PatchAsync([FromBody] UpdatePositionCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var position = await updatePositionHandler.Handle(command with { Id = id }, ct);

        return position is null ? NotFound() : Ok(position);
    }

    /// <summary>
    ///     Deletes a position (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        await deletePositionHandler.Handle(new DeletePositionCommand(id), ct);

        return NoContent();
    }
}