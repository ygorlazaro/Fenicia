using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class EmployeeController(IEmployeeService employeeService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of employees.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="query">Search query (optional).</param>
    /// <param name="sort">Sort field (optional).</param>
    /// <param name="filters">Additional filters (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of employees.</returns>
    /// <response code="200">List of employees returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<EmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<EmployeeResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        [FromQuery] Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await employeeService.GetAllAsync(
                new EmployeeQuery(page, perPage, query, sort) { Filters = filters },
                cancellationToken);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Employee details.</returns>
    /// <response code="200">Employee found.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeeResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.GetByIdAsync(id, cancellationToken);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="command">Employee data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created employee.</returns>
    /// <response code="201">Employee created successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authorized.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<EmployeeResponse>> PostAsync(
        [FromBody] EmployeeRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.AddAsync(command, companyContext.CompanyId, cancellationToken);

            return new CreatedResult(string.Empty, employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="command">Updated employee data.</param>
    /// <param name="id">Employee ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated employee.</returns>
    /// <response code="200">Employee updated successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authorized.</response>
    /// <response code="404">Employee not found.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<EmployeeResponse>> PatchAsync(
        [FromBody] EmployeeRequest command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.UpdateAsync(
                id,
                command,
                companyContext.CompanyId,
                cancellationToken);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Employee deleted successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await employeeService.DeleteAsync(
                id,
                companyContext.CompanyId,
                cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets employee performance data.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="days">Period in days for analysis (default: 90).</param>
    /// <param name="topLimit">Limit of top performers (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Employee performance data.</returns>
    /// <response code="200">Performance returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeePerformanceResponse>> GetPerformanceAsync(
        WideEventContext wide,
        [FromQuery] int days = 90,
        [FromQuery] int topLimit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await employeeService.GetPerformanceAsync(
                new EmployeePerformanceQuery(days, topLimit),
                cancellationToken);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}