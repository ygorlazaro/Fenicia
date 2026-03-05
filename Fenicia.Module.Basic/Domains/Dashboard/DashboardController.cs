using System.Net.Mime;

using Fenicia.Module.Basic.Domains.Dashboard.GetFinancialDashboard;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Dashboard;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DashboardController(
    GetFinancialDashboardHandler getFinancialDashboardHandler) : ControllerBase
{
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetFinancialDashboardAsync(
        [FromQuery] int days = 90,
        CancellationToken ct = default)
    {
        var dashboard = await getFinancialDashboardHandler.Handle(new GetFinancialDashboardQuery(days), ct);

        return Ok(dashboard);
    }
}
