using Fenicia.Common;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Module.Basic.Domains.Employee;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Position;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CustomerController(IPositionService positionService, IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var positions = await positionService.GetAllAsync(ct);

        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var position = await positionService.GetByIdAsync(id, ct);

        if (position is null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpGet("{id:guid}/employee")]
    public async Task<IActionResult> GetEmployeesByPositionIdAsync([FromRoute] Guid id, [FromQuery] PaginationQuery query, CancellationToken ct)
    {
        var employees = await employeeService.GetByPositionIdAsync(id, ct, query.Page, query.PerPage);

        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] PositionRequest request, CancellationToken ct)
    {
        var position = await positionService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, position);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] PositionRequest request, [FromRoute] Guid id, CancellationToken ct)
    {
        var position = await positionService.UpdateAsync(request, ct);

        if (position is null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await positionService.DeleteAsync(id, ct);

        return NoContent();
    }
}
