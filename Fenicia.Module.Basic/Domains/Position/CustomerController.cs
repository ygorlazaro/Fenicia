using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Employee.GetByPositionId;
using Fenicia.Module.Basic.Domains.Position.Add;
using Fenicia.Module.Basic.Domains.Position.Delete;
using Fenicia.Module.Basic.Domains.Position.GetAll;
using Fenicia.Module.Basic.Domains.Position.GetById;
using Fenicia.Module.Basic.Domains.Position.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Position;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CustomerController(
    GetAllPositionHandler getAllPositionHandler,
    GetPositionByIdHandler getPositionByIdHandler,
    AddPositionHandler addPositionHandler,
    UpdatePositionHandler updatePositionHandler,
    DeletePositionHandler deletePositionHandler,
    GetEmployeesByPositionIdHandler getEmployeesByPositionIdHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var positions = await getAllPositionHandler.Handle(new GetAllPositionQuery(page, perPage), ct);

        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var position = await getPositionByIdHandler.Handle(new GetPositionByIdQuery(id), ct);

        return position is null ? NotFound() : Ok(position);
    }

    [HttpGet("{id:guid}/employee")]
    public async Task<IActionResult> GetEmployeesByPositionIdAsync(
        [FromRoute] Guid id,
        [FromQuery] PaginationQuery query,
        CancellationToken ct)
    {
        var employees = await getEmployeesByPositionIdHandler.Handle(new GetEmployeesByPositionIdQuery(id, query.Page, query.PerPage), ct);

        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddPositionCommand command, CancellationToken ct)
    {
        var position = await addPositionHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, position);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync(
        [FromBody] UpdatePositionCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var position = await updatePositionHandler.Handle(command with { Id = id }, ct);

        return position is null ? NotFound() : Ok(position);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deletePositionHandler.Handle(new DeletePositionCommand(id), ct);

        return NoContent();
    }
}
