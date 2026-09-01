using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Report.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ReportController(ReportService reportService) : ControllerBase
{
    /// <summary>
    /// Creates a new report.
    /// </summary>
    /// <param name="command">Report data. Example: <c>{ "targetId": "22222222-2222-2222-2222-222222222222", "targetType": "Feed", "reason": "Spam", "description": "This feed contains spam content" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created report details.</returns>
    /// <response code="201">Report created successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "reporterId": "33333333-3333-3333-3333-333333333333", "targetId": "22222222-2222-2222-2222-222222222222", "targetType": "Feed", "reason": "Spam", "description": "This feed contains spam content", "status": "Pending", "reportDate": "2024-01-15T00:00:00Z" }</c></response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to create reports.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddReportResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddReportResponse>> PostAsync([FromBody] AddReportCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var report = await reportService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, report);
    }

    /// <summary>
    /// Updates the status of a report.
    /// </summary>
    /// <param name="id">The unique identifier of the report to update. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="command">The new status. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "status": "Approved" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The updated report status, or null if not found.</returns>
    /// <response code="200">Report status updated successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "status": "Approved" }</c></response>
    /// <response code="400">Invalid status value supplied. Status must be Approved or Denied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to update reports.</response>
    /// <response code="404">Report with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="ArgumentException">Thrown by the service when the status is not Approved or Denied.</exception>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database update.</exception>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateReportResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateReportResponse>> PatchStatusAsync([FromRoute] Guid id, [FromBody] UpdateReportStatusCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await reportService.UpdateStatusAsync(command with { Id = id }, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Gets all reports with pagination.
    /// </summary>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="query"></param>
    /// <param name="sort"></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>A list of reports for the requested page.</returns>
    /// <response code="200">Reports retrieved successfully. Example: <c>[{ "id": "11111111-1111-1111-1111-111111111111", "reporterId": "33333333-3333-3333-3333-333333333333", "targetId": "22222222-2222-2222-2222-222222222222", "targetType": "Feed", "reason": "Spam", "description": "This feed contains spam content", "status": "Pending", "reportDate": "2024-01-15T00:00:00Z" }]</c></response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to view reports.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllReportResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllReportResponse>>> GetAllAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var reports = await reportService.GetAllAsync(new GetAllReportQuery(page, perPage, query, sort), cancellationToken);

        return Ok(reports);
    }

    /// <summary>
    /// Gets a report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The report details, or null if not found.</returns>
    /// <response code="200">Report found. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "reporterId": "33333333-3333-3333-3333-333333333333", "targetId": "22222222-2222-2222-2222-222222222222", "targetType": "Feed", "reason": "Spam", "description": "This feed contains spam content", "status": "Pending", "reportDate": "2024-01-15T00:00:00Z" }</c></response>
    /// <response code="400">Invalid ID format supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="404">Report with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetReportByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetReportByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var report = await reportService.GetByIdAsync(new GetReportByIdQuery(id), cancellationToken);

        return report is null ? NotFound() : Ok(report);
    }
}
