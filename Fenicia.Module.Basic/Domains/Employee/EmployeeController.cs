using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Employee.Commands;
using Fenicia.Module.Basic.Domains.Employee.Handlers;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

/// <summary>
///     Controller responsible for handling employee-related HTTP endpoints.
///     Provides CRUD operations and employee performance analytics.
/// </summary>
/// <remarks>
///     All endpoints require authentication.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class EmployeeController(GetAllEmployeeHandler getAllEmployeeHandler, GetEmployeeByIdHandler getEmployeeByIdHandler, AddEmployeeHandler addEmployeeHandler, UpdateEmployeeHandler updateEmployeeHandler, DeleteEmployeeHandler deleteEmployeeHandler, GetEmployeePerformanceHandler getEmployeePerformanceHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of all employees.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of employees.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllEmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllEmployeeResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var employees = await getAllEmployeeHandler.Handle(new GetAllEmployeeQuery(page, perPage), ct);

        return Ok(employees);
    }

    /// <summary>
    ///     Retrieves a specific employee by ID.
    /// </summary>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Employee details or 404 if not found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetEmployeeByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var employee = await getEmployeeByIdHandler.Handle(new GetEmployeeByIdQuery(id), ct);

        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    ///     Creates a new employee.
    /// </summary>
    /// <param name="command">Employee creation command.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created employee.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddEmployeeResponse>> PostAsync([FromBody] AddEmployeeCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var employee = await addEmployeeHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, employee);
    }

    /// <summary>
    ///     Updates an existing employee.
    /// </summary>
    /// <param name="command">Employee update command.</param>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated employee or 404 if not found.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateEmployeeResponse>> PatchAsync([FromBody] UpdateEmployeeCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var employee = await updateEmployeeHandler.Handle(command with { Id = id }, ct);

        return employee is null ? NotFound() : Ok(employee);
    }

    /// <summary>
    ///     Deletes an employee (soft delete).
    /// </summary>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        await deleteEmployeeHandler.Handle(new DeleteEmployeeCommand(id), ct);

        return NoContent();
    }

    /// <summary>
    ///     Retrieves employee performance analytics.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topLimit">Number of top performers to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Employee performance data including top performers.</returns>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeePerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var performance = await getEmployeePerformanceHandler.Handle(new GetEmployeePerformanceQuery(days, topLimit), ct);

        return Ok(performance);
    }
}