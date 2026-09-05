using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

/// <inheritdoc />
/// <summary>
///     Gerencia operações de perfis de usuário.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public sealed class ProfileController(IProfileService profileService) : ControllerBase
{
    /// <summary>
    ///     Obtém um perfil pelo ID.
    /// </summary>
    /// <param name="id">ID do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do perfil</returns>
    /// <response code="200">Perfil encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Perfil não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o perfil</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetProfileByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProfileByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profile = await profileService.GetByIdAsync(new GetProfileByIdQuery(id), cancellationToken);

        return profile is null ? NotFound() : Ok(profile);
    }

    /// <summary>
    ///     Obtém o perfil do usuário autenticado.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do perfil</returns>
    /// <response code="200">Perfil encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Perfil não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o perfil</exception>
    [HttpGet]
    [ProducesResponseType(typeof(GetProfileByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProfileByIdResponse>> GetAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        var userId = ClaimReader.UserId(User);
        wide.UserId = userId.ToString();

        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken);

        return profile is null ? NotFound() : Ok(profile);
    }

    /// <summary>
    ///     Cria o perfil do usuário autenticado caso ainda não exista.
    /// </summary>
    /// <param name="command">Dados iniciais do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil criado (ou existente)</returns>
    /// <response code="201">Perfil criado com sucesso</response>
    /// <response code="200">Perfil já existia, retornado o existente</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AddProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AddProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddProfileResponse>> PostAsync(
        [FromBody] AddProfileCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        var userId = ClaimReader.UserId(User);
        wide.UserId = userId.ToString();

        var existing = await profileService.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return Ok(new AddProfileResponse(
                existing.Id,
                existing.UserId,
                existing.Bio,
                existing.ImageUrl,
                existing.Website,
                existing.Location,
                existing.Phone,
                existing.BirthDate));
        }

        var created = await profileService.CreateAsync(command, userId, cancellationToken);
        return new CreatedResult(string.Empty, created);
    }

    /// <summary>
    ///     Atualiza o perfil do usuário autenticado.
    /// </summary>
    /// <param name="id">ID do perfil</param>
    /// <param name="command">Dados atualizados do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil atualizado</returns>
    /// <response code="200">Perfil atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UpdateProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProfileResponse>> PatchAsync(
        [FromBody] UpdateProfileCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profile = await profileService.UpdateAsync(
            command with { Id = id },
            ClaimReader.UserId(User),
            cancellationToken);

        return profile switch
        {
            null => NotFound(),
            _ => Ok(profile)
        };
    }
}
