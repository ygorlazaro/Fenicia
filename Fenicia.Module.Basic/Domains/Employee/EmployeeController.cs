using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

[Authorize]
[ApiController]
[Route("[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var employees = await employeeService.GetAllAsync(ct);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await employeeService.GetByIdAsync(id, ct);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] EmployeeRequest request, CancellationToken ct)
    {
        var employee = await employeeService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, employee);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] EmployeeRequest request, [FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await employeeService.UpdateAsync(request, ct);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await employeeService.DeleteAsync(id, ct);

        return NoContent();
    }
}
