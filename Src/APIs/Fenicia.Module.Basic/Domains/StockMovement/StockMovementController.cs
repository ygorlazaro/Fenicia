using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[ApiController]
[Authorize]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StockMovementController(IStockMovementService stockMovementService) : ControllerBase
{
    /// <summary>
    ///     Obtém movimentações de estoque por período.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Consulta avançada para filtros</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de movimentações de estoque</returns>
    /// <response code="200">Movimentações retornadas com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetStockMovementResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetStockMovementResponse>>> GetAsync(
        WideEventContext wide,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var stockMovement = await stockMovementService.GetAsync(
                new GetStockMovementQuery(startDate, endDate, page, perPage, query, sort),
                cancellationToken);

            return Ok(stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Cria uma nova movimentação de estoque.
    /// </summary>
    /// <param name="command">Dados da movimentação</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Movimentação criada</returns>
    /// <response code="201">Movimentação criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddStockMovementResponse>> PostAsync(
        [FromBody] AddStockMovementCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = ClaimReader.UserId(User);
            var stockMovement = await stockMovementService.AddAsync(command, companyId, cancellationToken);

            return new CreatedResult(string.Empty, stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Atualiza uma movimentação de estoque existente.
    /// </summary>
    /// <param name="id">ID da movimentação</param>
    /// <param name="command">Dados atualizados da movimentação</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Movimentação atualizada</returns>
    /// <response code="200">Movimentação atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Acesso negado</response>
    /// <response code="404">Movimentação não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateStockMovementResponse>> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateStockMovementCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = ClaimReader.UserId(User);
            var stockMovement = await stockMovementService.UpdateAsync(
                command with { Id = id },
                companyId,
                cancellationToken);

            return stockMovement is null ? NotFound() : Ok(stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Obtém métricas analíticas das movimentações de estoque.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Período em dias para análise</param>
    /// <param name="topLimit">Limite de produtos no top</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Métricas analíticas das movimentações</returns>
    /// <response code="200">Análise retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockMovementDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StockMovementDashboardResponse>> GetDashboardAsync(
        WideEventContext wide,
        [FromQuery] int days = 30,
        [FromQuery] int topLimit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await stockMovementService.GetDashboardAsync(
                new GetStockMovementDashboardQuery(days, topLimit),
                cancellationToken);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}