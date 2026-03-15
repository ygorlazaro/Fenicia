using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Dashboard.Handlers;
using Fenicia.Module.Basic.Domains.Dashboard.Queries;
using Fenicia.Module.Basic.Domains.Dashboard.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Dashboard;

/// <summary>
///     Controller responsible for handling dashboard-related HTTP endpoints.
///     Provides access to financial dashboard analytics and KPIs.
/// </summary>
/// <remarks>
///     All endpoints require authentication.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DashboardController(GetFinancialDashboardHandler getFinancialDashboardHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves the financial dashboard with KPIs, revenue analysis, and sales summaries.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Comprehensive financial dashboard data.</returns>
    /// <response code="200">Returns the financial dashboard successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetFinancialDashboardAsync(WideEventContext wide, [FromQuery] int days = 90, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(this.User).ToString();

            var dashboard = await getFinancialDashboardHandler.Handle(new GetFinancialDashboardQuery(days), ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}