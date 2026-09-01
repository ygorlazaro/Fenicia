using System.Net.Mime;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Cria um novo pedido com os módulos informados.
    /// </summary>
    /// <param name="request">Comando com lista de IDs de módulos</param>
    /// <param name="headers">Cabeçalhos da requisição (inclui CompanyId)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do pedido criado</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Requisição inválida (ex: módulos não encontrados)</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não associado à empresa</response>
    /// <response code="404">Empresa ou módulos não encontrados</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewOrderResponse>> CreateNewOrderAsync(CreateNewOrderCommand request, [FromHeader] Headers headers, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var userId = ClaimReader.UserId(User);
            var companyId = headers.CompanyId;
            var command = new CreateNewOrderCommand(userId, companyId, request.Modules);
            var order = await orderService.CreateAsync(command, cancellationToken);

            return order switch
            {
                null => BadRequest(),
                _ => Created(string.Empty, order)
            };
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (PermissionDeniedException)
        {
            return Forbid();
        }
        catch (ItemNotExistsException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
