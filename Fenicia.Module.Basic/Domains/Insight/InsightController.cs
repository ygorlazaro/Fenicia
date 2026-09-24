using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Module.Basic.Domains.Insight.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Insight;

/// <summary>
/// Controller responsible for handling insight-related operations.
/// </summary>
/// <param name="insightService">The insight service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InsightController(IInsightService insightService) : ControllerBase
{
    /// <summary>
    /// Gets aggregated customer insights.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="request">The customer insights request containing filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer insights.</returns>
    /// <response code="200">Insights returned successfully.</response>
    /// <response code="400">Invalid parameters.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("customer")]
    [ProducesResponseType(typeof(CustomerInsightsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerInsightsResponse>> GetCustomerInsightsAsync(
        WideEventContext wide,
        [FromQuery] CustomerInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var insights = await insightService.GetCustomerInsightsAsync(request, cancellationToken);

        return Ok(insights);
    }
}