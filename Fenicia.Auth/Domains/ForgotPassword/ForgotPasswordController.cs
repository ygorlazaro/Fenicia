using System.Net.Mime;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.ForgotPassword;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController(ForgotPasswordService forgotPasswordService) : ControllerBase
{
    /// <summary>
    /// Inicia o fluxo de recuperação de senha enviando um código para o e-mail informado.
    /// </summary>
    /// <param name="reset">Comando com o e-mail do usuário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (201) se o código foi gerado com sucesso</returns>
    /// <response code="201">Código de recuperação gerado e enviado</response>
    /// <response code="400">E-mail não encontrado ou inválido</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] AddForgotPasswordCommand reset, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = reset.Email;

            var ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = HttpContext?.Request?.Headers.UserAgent.ToString();

            var command = new AddForgotPasswordCommand(reset.Email, ipAddress, userAgent);

            await forgotPasswordService.AddAsync(command, ct);

            return Created();
        }
        catch (ItemNotExistsException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Redefine a senha do usuário usando o código de recuperação enviado por e-mail.
    /// </summary>
    /// <param name="request">Comando com e-mail, nova senha e código de recuperação</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (201) se a senha foi redefinida com sucesso</returns>
    /// <response code="201">Senha redefinida com sucesso</response>
    /// <response code="400">Código inválido, e-mail não encontrado ou senha inválida</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            await forgotPasswordService.ResetAsync(request, ct);

            return Created();
        }
        catch (ItemNotExistsException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
