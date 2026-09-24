using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

/// <summary>
/// Controller responsible for handling employee-related operations.
/// </summary>
/// <param name="employeeService">The employee service.</param>
/// <param name="companyContext">The company context.</param>
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
    /// <param name="query">The pagination and filter query.</param>
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
        [FromQuery] EmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await employeeService.GetAllAsync(query, cancellationToken);

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
    /// <param name="command">Employee data.</param>
    /// <param name="wide">Wide event context.</param>
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

            return CreatedAtAction(nameof(GetByIdAsync), new { id = employee.Id }, employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">Employee ID.</param>
    /// <param name="command">Updated employee data.</param>
    /// <param name="wide">Wide event context.</param>
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
        [FromRoute] Guid id,
        [FromBody] EmployeeRequest command,
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
    /// <param name="request">The performance request parameters.</param>
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
        [FromQuery] EmployeePerformanceRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await employeeService.GetPerformanceAsync(request, cancellationToken);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets employees by position ID.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="positionId"></param>
    /// <param name="query">The query containing position ID and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of employees.</returns>
    /// <response code="200">Employees returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("position/{positionId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<EmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<EmployeeResponse>>>> GetByPositionIdAsync(
        WideEventContext wide,
        [FromRoute] Guid positionId,
        [FromQuery] EmployeesByPositionIdRequest query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var request = query with { PositionId = positionId };
            var employees = await employeeService.GetByPositionIdAsync(request, cancellationToken);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
