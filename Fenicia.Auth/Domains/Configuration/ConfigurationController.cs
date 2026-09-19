using System.Net.Mime;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Configuration;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ConfigurationController(IConfigurationService configurationService) : ControllerBase
{
    /// <summary>
    ///     Obtém todas as configurações de um usuário para uma empresa.
    /// </summary>
    /// <param name="companyId">ID da empresa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de configurações do usuário para a empresa</returns>
    /// <response code="200">Configurações encontradas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar configurações desta empresa</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ConfigurationResponse>>> GetAsync(
        [FromQuery] Guid companyId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

            var result = await configurationService.GetAllAsync(userId, companyId, cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Cria ou atualiza uma configuração (upsert) para o usuário autenticado.
    /// </summary>
    /// <param name="companyId">ID da empresa</param>
    /// <param name="request">Dados da configuração (tipo, valor)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se criada/atualizada com sucesso</returns>
    /// <response code="204">Configuração criada ou atualizada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para esta empresa</response>
    /// <response code="500">Erro interno do servidor</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> PatchAsync(
        [FromQuery] Guid companyId,
        [FromBody] ConfigurationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            request.UserId = userId;

            await configurationService.UpsertAsync(request, companyId, cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}