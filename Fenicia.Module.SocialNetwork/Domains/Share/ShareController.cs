using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.SocialNetwork.Share;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

/// <summary>
///     Gerencia operações de compartilhamento de feeds.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ShareController(
    IShareService shareService,
    ICompanyContext companyContext,
    IProfileService profileService) : ControllerBase
{
/// <summary>
///     Compartilha um feed.
/// </summary>
/// <param name="command">Dados do compartilhamento</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Compartilhamento criado</returns>
/// <response code="201">Compartilhamento criado com sucesso</response>
/// <response code="400">Dados inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddShareResponse))]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Consumes(MediaTypeNames.Application.Json)]
public async Task<ActionResult<AddShareResponse>> PostAsync(
        [FromBody] ShareCommand command,
        CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var share = await shareService.ShareAsync(
            command,
            companyContext.CompanyId,
            profileId,
            cancellationToken);

        return new CreatedResult(string.Empty, share);
    }

/// <summary>
///     Obtém os compartilhamentos de um feed.
/// </summary>
/// <param name="feedId">ID do feed</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="page">Página</param>
/// <param name="perPage">Itens por página</param>
/// <param name="query">Termo de busca</param>
/// <param name="sort">Ordenação</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Lista de compartilhamentos</returns>
/// <response code="200">Lista de compartilhamentos</response>
/// <response code="400">Parâmetros inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpGet("feed/{feedId:guid}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSharesResponse>))]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<List<GetSharesResponse>>> GetSharesByFeedAsync(
        [FromRoute] Guid feedId,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {

        var shares = await shareService.GetSharesByFeedAsync(
            new GetSharesByFeedQuery(page, perPage, query, sort),
            feedId,
            cancellationToken);

        return Ok(shares);
    }

    private async Task<Guid> GetCurrentProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken)
                      ?? throw new InvalidOperationException("Perfil social não encontrado para o usuário atual.");
        return profile.Id;
    }
}
