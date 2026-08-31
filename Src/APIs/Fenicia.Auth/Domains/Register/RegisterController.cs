using System.Net.Mime;
using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(RegisterService registerService) : ControllerBase
{
    /// <summary>
    /// Cria um novo usuário com sua empresa inicial.
    /// </summary>
    /// <param name="request">Dados do usuário (e-mail, senha, nome, empresa)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário e empresa criados</returns>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">E-mail já existe, empresa já existe ou dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<RegisterResponse>> CreateNewUserAsync(RegisterCommand request, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await registerService.CreateAsync(request, cancellationToken);

            return Created(string.Empty, userResponse);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
