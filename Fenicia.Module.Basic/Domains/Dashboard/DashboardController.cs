using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Basic.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Dashboard;

/// <summary>
/// Controller responsible for handling dashboard-related operations.
/// </summary>
/// <param name="dashboardService">The dashboard service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    /// Gets the financial dashboard with KPIs, revenue vs cost, profit margin, accounts receivable and daily sales.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="request">The financial dashboard request containing filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The financial dashboard response.</returns>
    /// <response code="200">Financial dashboard returned successfully.</response>
    /// <response code="400">Invalid request parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetFinancialDashboardAsync(
        WideEventContext wide,
        [FromQuery] FinancialDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await dashboardService.GetFinancialDashboardAsync(request, cancellationToken);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}