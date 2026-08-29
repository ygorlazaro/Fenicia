using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(OrderService orderService) : ControllerBase
{
    /// <summary>
    /// Obtém uma lista paginada de pedidos.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de pedidos</returns>
    /// <response code="200">Lista de pedidos retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllOrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllOrderResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var orders = await orderService.GetAllAsync(new GetAllOrderQuery(page, perPage), ct);

            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém um pedido pelo ID.
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do pedido</returns>
    /// <response code="200">Pedido encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrderByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetOrderByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var order = await orderService.GetByIdAsync(new GetOrderByIdQuery(id), ct);

            return order is null ? NotFound() : Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    /// <param name="command">Dados do pedido a ser criado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Pedido criado</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateOrderResponse>> PostAsync([FromBody] CreateOrderCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var userId = ClaimReader.UserId(User);
            var order = await orderService.CreateAsync(command with { UserId = userId }, ClaimReader.UserId(User), ct);

            return new CreatedResult(string.Empty, order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Remove um pedido (soft delete).
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <response code="204">Pedido removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Acesso negado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await orderService.DeleteAsync(new DeleteOrderCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém métricas analíticas dos pedidos.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Período em dias para análise</param>
    /// <param name="topCustomersLimit">Limite de clientes no top</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Métricas analíticas dos pedidos</returns>
    /// <response code="200">Análise retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderAnalyticsResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderAnalyticsResponse>> GetAnalyticsAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topCustomersLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var analytics = await orderService.GetAnalyticsAsync(new GetOrderAnalyticsQuery(days, topCustomersLimit), ct);

            return Ok(analytics);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
