using System.Net.Mime;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Subscription;

/// <summary>
/// Controller for managing subscription-related operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SubscriptionController(ISubscriptionService subscriptionService) : ControllerBase
{
    /// <summary>
    /// Gets the authenticated user's profile with companies and subscriptions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile with companies and subscriptions</returns>
    /// <response code="200">Profile found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponse>> GetUserProfile(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

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