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
    /// <response code="200">Employees retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllEmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllEmployeeResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await getAllEmployeeHandler.Handle(new GetAllEmployeeQuery(page, perPage), ct);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a specific employee by ID.
    /// </summary>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Employee details or 404 if not found.</returns>
    /// <response code="200">Employee found.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Employee not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetEmployeeByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await getEmployeeByIdHandler.Handle(new GetEmployeeByIdQuery(id), ct);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new employee.
    /// </summary>
    /// <param name="command">Employee creation command.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created employee.</returns>
    /// <response code="201">Employee created successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddEmployeeResponse>> PostAsync([FromBody] AddEmployeeCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await addEmployeeHandler.Handle(command, ct);

            return new CreatedResult(string.Empty, employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing employee.
    /// </summary>
    /// <param name="command">Employee update command.</param>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated employee or 404 if not found.</returns>
    /// <response code="200">Employee updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Employee not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateEmployeeResponse>> PatchAsync([FromBody] UpdateEmployeeCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await updateEmployeeHandler.Handle(command with { Id = id }, ct);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Deletes an employee (soft delete).
    /// </summary>
    /// <param name="id">Employee's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Employee deleted successfully.</response>
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

            await deleteEmployeeHandler.Handle(new DeleteEmployeeCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves employee performance analytics.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topLimit">Number of top performers to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Employee performance data including top performers.</returns>
    /// <response code="200">Performance data retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeePerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await getEmployeePerformanceHandler.Handle(new GetEmployeePerformanceQuery(days, topLimit), ct);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
