using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Employee.Add;
using Fenicia.Module.Basic.Domains.Employee.Delete;
using Fenicia.Module.Basic.Domains.Employee.GetAll;
using Fenicia.Module.Basic.Domains.Employee.GetById;
using Fenicia.Module.Basic.Domains.Employee.GetEmployeePerformance;
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
    DeleteEmployeeHandler deleteEmployeeHandler,
    GetEmployeePerformanceHandler getEmployeePerformanceHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllEmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllEmployeeResponse>>>> GetAsync(
        [FromQuery] int page = 1, 
        [FromQuery] int perPage = 10, 
        CancellationToken ct = default)
    {
        var employees = await getAllEmployeeHandler.Handle(new GetAllEmployeeQuery(page, perPage), ct);

        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetEmployeeByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var employee = await getEmployeeByIdHandler.Handle(new GetEmployeeByIdQuery(id), ct);

        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddEmployeeResponse>> PostAsync([FromBody] AddEmployeeCommand command, CancellationToken ct)
    {
        var employee = await addEmployeeHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, employee);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateEmployeeResponse>> PatchAsync(
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
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteEmployeeHandler.Handle(new DeleteEmployeeCommand(id), ct);

        return NoContent();
    }

    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeePerformanceResponse>> GetPerformanceAsync(
        [FromQuery] int days = 90,
        [FromQuery] int topLimit = 10,
        CancellationToken ct = default)
    {
        var performance = await getEmployeePerformanceHandler.Handle(new GetEmployeePerformanceQuery(days, topLimit), ct);

        return Ok(performance);
    }
}
