using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.SocialNetwork.Comment;
using Fenicia.Module.SocialNetwork.Domains.Comment.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

/// <summary>
///     Gerencia operações de comentários em feeds.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CommentController(
    ICommentService commentService,
    ICompanyContext companyContext,
    IProfileService profileService) : ControllerBase
{
    /// <summary>
    ///     Obtém os comentários de um feed.
    /// </summary>
    /// <param name="feedId">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Termo de busca</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de comentários</returns>
    /// <response code="200">Lista de comentários</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCommentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCommentResponse>>> GetByFeedAsync(
            [FromRoute] Guid feedId,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? query = null,
            [FromQuery] string? sort = null,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.GetAllByFeedAsync(
            new GetAllCommentByFeedQuery(page, perPage, feedId, query, sort),
            feedId,
            profileId,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Obtém um comentário pelo ID.
    /// </summary>
    /// <param name="id">ID do comentário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Comentário encontrado</returns>
    /// <response code="200">Comentário encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Comentário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCommentByIdResponse>> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
    {

        var result = await commentService.GetByIdAsync(new GetCommentByIdQuery(id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    ///     Cria um novo comentário.
    /// </summary>
    /// <param name="command">Dados do comentário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Comentário criado</returns>
    /// <response code="201">Comentário criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCommentResponse>> PostAsync(
            [FromBody] AddCommentCommand command,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var addCommand = new AddCommentCommand(
            command.Id,
            profileId,
            command.FeedId,
            command.ParentCommentId,
            command.Text);
        var result = await commentService.AddAsync(
            addCommand,
            companyContext.CompanyId,
            profileId,
            cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    ///     Atualiza um comentário.
    /// </summary>
    /// <param name="command">Dados atualizados do comentário</param>
    /// <param name="id">ID do comentário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Comentário atualizado</returns>
    /// <response code="200">Comentário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Comentário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCommentResponse>> PatchAsync(
            [FromBody] UpdateCommentCommand command,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var updatedCommand = new UpdateCommentCommand(id, command.Text);
        var result = await commentService.UpdateAsync(
            updatedCommand,
            profileId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    ///     Remove um comentário.
    /// </summary>
    /// <param name="id">ID do comentário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Comentário removido com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        await commentService.DeleteAsync(new DeleteCommentCommand(id), profileId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Obtém as respostas de um comentário.
    /// </summary>
    /// <param name="parentCommentId">ID do comentário pai</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Termo de busca</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de respostas</returns>
    /// <response code="200">Lista de respostas</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("replies/{parentCommentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRepliesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetRepliesResponse>>> GetRepliesAsync(
            [FromRoute] Guid parentCommentId,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? query = null,
            [FromQuery] string? sort = null,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.GetRepliesAsync(
            new GetRepliesQuery(page, perPage, parentCommentId, query, sort),
            profileId,
            cancellationToken);

        return Ok(result);
    }

    private async Task<Guid> GetCurrentProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken)
                      ?? throw new InvalidOperationException("Perfil social não encontrado para o usuário atual.");
        return profile.Id;
    }
}
