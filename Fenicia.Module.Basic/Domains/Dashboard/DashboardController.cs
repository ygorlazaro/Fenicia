using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Dashboard;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DashboardController(DashboardService dashboardService) : ControllerBase
{

    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetFinancialDashboardAsync(WideEventContext wide, [FromQuery] int days = 90, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await dashboardService.GetFinancialDashboardAsync(new GetFinancialDashboardQuery(days), ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
