using System.Net.Mime;

using Fenicia.Module.Basic.Domains.Employee.Add;
using Fenicia.Module.Basic.Domains.Employee.Delete;
using Fenicia.Module.Basic.Domains.Employee.GetAll;
using Fenicia.Module.Basic.Domains.Employee.GetById;
using Fenicia.Module.Basic.Domains.Employee.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class EmployeeController(
    GetAllEmployeeHandler getAllEmployeeHandler,
    GetEmployeeByIdHandler getEmployeeByIdHandler,
    AddEmployeeHandler addEmployeeHandler,
    UpdateEmployeeHandler updateEmployeeHandler,
    DeleteEmployeeHandler deleteEmployeeHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EmployeeResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeeResponse>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var employees = await getAllEmployeeHandler.Handle(new GetAllEmployeeQuery(page, perPage), ct);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeeResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await getEmployeeByIdHandler.Handle(new GetEmployeeByIdQuery(id), ct);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<EmployeeResponse>> PostAsync([FromBody] AddEmployeeCommand command, CancellationToken ct)
    {
        var employee = await addEmployeeHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, employee);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<EmployeeResponse>> PatchAsync(
        [FromBody] UpdateEmployeeCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var employee = await updateEmployeeHandler.Handle(command with { Id = id }, ct);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeeResponse>> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteEmployeeHandler.Handle(new DeleteEmployeeCommand(id), ct);

        return NoContent();
    }
}
