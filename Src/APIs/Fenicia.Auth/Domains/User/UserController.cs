using System.Net.Mime;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController(UserService userService, ModuleService moduleService) : ControllerBase
{
    /// <summary>
    /// Obtém os módulos de um usuário para uma empresa.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="headers">Cabeçalhos da requisição (inclui CompanyId)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de módulos do usuário na empresa</returns>
    /// <response code="200">Módulos encontrados</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar módulos desta empresa</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}/module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserModulesResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<GetUserModulesResponse>>> GetUserModulesAsync([FromRoute] Guid id, [FromHeader] Headers headers, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            var loggedInUserId = ClaimReader.UserId(User);
            wide.UserId = loggedInUserId.ToString();

            await userService.EnsureCanAccessUserAsync(loggedInUserId, id, headers.CompanyId, cancellationToken);

            var companyId = headers.CompanyId;
            var response = await moduleService.GetUserModulesAsync(companyId, id, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém as empresas associadas a um usuário.
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de empresas do usuário</returns>
    /// <response code="200">Empresas encontradas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar empresas deste usuário</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}/company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            var loggedInUserId = ClaimReader.UserId(User);
            wide.UserId = loggedInUserId.ToString();

            await userService.EnsureCanAccessUserAsync(loggedInUserId, id, null, cancellationToken);

            var response = await userService.GetCompaniesAsync(id, cancellationToken);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém todos os usuários com paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Quantidade de itens por página (padrão: 10)</param>
    /// <param name="query">Filtros avançados. Example: <c>name[*]alpha</c></param>
    /// <param name="sort">Ordenação. Example: <c>name</c></param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários</returns>
    /// <response code="200">Lista de usuários retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        var result = await userService.GetAllAsync(new GetAllUsersQuery(page, pageSize, query, sort), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);

        return user switch
        {
            null => NotFound(),
            _ => Ok(user)
        };
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="request">Dados do usuário (e-mail, senha, nome, roles)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário criado</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">E-mail já existe ou dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await userService.CreateAsync(request, cancellationToken);

            return Created(string.Empty, result);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um usuário existente.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="request">Dados atualizados do usuário (nome, e-mail, roles por empresa)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário atualizado</returns>
    /// <response code="200">Usuário atualizado com sucesso</response>
    /// <response code="400">E-mail já existe ou dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "God,Admin")]
    public async Task<IActionResult> UpdateAsync(Guid userId, UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await userService.UpdateAsync(updateRequest, cancellationToken);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Remove um usuário (soft delete).
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se removido com sucesso</returns>
    /// <response code="204">Usuário removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await userService.DeleteAsync(userId, cancellationToken);
            return NoContent();
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Altera a senha de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="request">Comando com a nova senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de alteração de senha</returns>
    /// <response code="200">Senha alterada com sucesso</response>
    /// <response code="400">Senha inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{userId:guid}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangePasswordAsync(Guid userId, UpdateUserPasswordCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            var updateRequest = request with { UserId = userId };
            var result = await userService.UpdatePasswordAsync(updateRequest, cancellationToken);

            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            return NotFound();
        }
    }
}
