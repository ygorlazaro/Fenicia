using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InventoryController(InventoryService inventoryService) : ControllerBase
{
    /// <summary>
    /// Obtém o inventário por produto.
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do inventário do produto</returns>
    /// <response code="200">Inventário retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByProductIdAsync([FromRoute] Guid productId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetByProductAsync(new GetInventoryByProductQuery(productId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém o inventário por categoria.
    /// </summary>
    /// <param name="categoryId">ID da categoria</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do inventário da categoria</returns>
    /// <response code="200">Inventário retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetByCategoryAsync(new GetInventoryByCategoryQuery(categoryId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém o inventário completo.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do inventário</returns>
    /// <response code="200">Inventário retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetAsync(new GetInventoryQuery(page, perPage), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém o dashboard do inventário.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do dashboard do inventário</returns>
    /// <response code="200">Dashboard retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryDashboardResponse>> GetInventoryDashboardAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await inventoryService.GetDashboardAsync(new GetInventoryDashboardQuery(), ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém a saúde do inventário.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="zeroMovementDays">Dias sem movimento para considerar produto parado</param>
    /// <param name="overstockMultiplier">Multiplicador para identificar excesso de estoque</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados de saúde do inventário</returns>
    /// <response code="200">Saúde do inventário retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryHealthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryHealthResponse>> GetInventoryHealthAsync(WideEventContext wide, [FromQuery] int zeroMovementDays = 90, [FromQuery] double overstockMultiplier = 3.0, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var health = await inventoryService.GetHealthAsync(new GetInventoryHealthQuery(zeroMovementDays, overstockMultiplier), ct);

            return Ok(health);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
