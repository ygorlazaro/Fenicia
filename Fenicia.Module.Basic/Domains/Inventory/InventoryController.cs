using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InventoryController(ISender sender) : ControllerBase
{

    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByProductIdAsync([FromRoute] Guid productId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await sender.Send(new GetInventoryByProductQuery(productId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await sender.Send(new GetInventoryByCategoryQuery(categoryId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await sender.Send(new GetInventoryQuery(page, perPage), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryDashboardResponse>> GetInventoryDashboardAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await sender.Send(new GetInventoryDashboardQuery(), ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryHealthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryHealthResponse>> GetInventoryHealthAsync(WideEventContext wide, [FromQuery] int zeroMovementDays = 90, [FromQuery] double overstockMultiplier = 3.0, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var health = await sender.Send(new GetInventoryHealthQuery(zeroMovementDays, overstockMultiplier), ct);

            return Ok(health);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
