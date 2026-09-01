using System.Net.Mime;
using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Subscription;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SubscriptionController(ISubscriptionService subscriptionService) : ControllerBase
{
    /// <summary>
    /// Obtém o perfil do usuário autenticado com empresas e assinaturas.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Perfil do usuário com empresas e assinaturas</returns>
    /// <response code="200">Perfil encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile(WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var profile = await subscriptionService.GetUserProfileAsync(userId, cancellationToken);

            return profile switch
            {
                null => NotFound(),
                _ => Ok(profile)
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
