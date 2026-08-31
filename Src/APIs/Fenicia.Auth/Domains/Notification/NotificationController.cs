using System.Net.Mime;
using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Notification;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class NotificationController(NotificationService notificationService) : ControllerBase
{
    /// <summary>
    /// Obtém todas as notificações do usuário autenticado com paginação.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="perPage">Quantidade de itens por página (padrão: 10)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de notificações</returns>
    /// <response code="200">Notificações encontradas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllNotificationsResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllNotificationsResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var notifications = await notificationService.GetAllAsync(page, perPage, ct);
            return Ok(notifications);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém uma notificação específica pelo ID.
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados da notificação</returns>
    /// <response code="200">Notificação encontrada</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Notificação não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetNotificationByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetNotificationByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var notification = await notificationService.GetByIdAsync(id, ct);
            return notification is null ? NotFound() : Ok(notification);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova notificação.
    /// </summary>
    /// <param name="command">Dados da notificação (título, descrição, data, imagem)</param>
    /// <param name="headers">Cabeçalhos da requisição (inclui CompanyId)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados da notificação criada</returns>
    /// <response code="201">Notificação criada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddNotificationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddNotificationResponse>> PostAsync([FromBody] AddNotificationCommand command, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var notification = await notificationService.AddAsync(command, headers.CompanyId, ct);
            return new CreatedResult(string.Empty, notification);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza uma notificação existente.
    /// </summary>
    /// <param name="command">Dados atualizados da notificação (título, descrição, data, imagem, lida)</param>
    /// <param name="id">ID da notificação</param>
    /// <param name="headers">Cabeçalhos da requisição (inclui CompanyId)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados da notificação atualizada</returns>
    /// <response code="200">Notificação atualizada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Notificação não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateNotificationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateNotificationResponse>> PatchAsync([FromBody] UpdateNotificationCommand command, [FromRoute] Guid id, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var notification = await notificationService.UpdateAsync(command with { Id = id }, headers.CompanyId, ct);
            return notification switch
            {
                null => NotFound(),
                _ => Ok(notification)
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Remove uma notificação (soft delete).
    /// </summary>
    /// <param name="id">ID da notificação</param>
    /// <param name="headers">Cabeçalhos da requisição (inclui CompanyId)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se removida com sucesso</returns>
    /// <response code="204">Notificação removida com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            await notificationService.DeleteAsync(id, headers.CompanyId, ct);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
