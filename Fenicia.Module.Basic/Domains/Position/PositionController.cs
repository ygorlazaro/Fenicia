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
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var positions = await positionService.GetAllAsync(cancellationToken);

        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var position = await positionService.GetByIdAsync(id, cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpGet("{id:guid}/employee")]
    public async Task<IActionResult> GetEmployeesByPositionIdAsync([FromRoute] Guid id, [FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetByPositionIdAsync(id, cancellationToken, query.Page, query.PerPage);

        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] PositionRequest request, CancellationToken cancellationToken)
    {
        var position = await positionService.AddAsync(request, cancellationToken);

        return new CreatedResult(string.Empty, position);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] PositionRequest request, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var position = await positionService.UpdateAsync(request, cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await positionService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
