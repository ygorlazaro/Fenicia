using System.Net.Mime;

using Fenicia.Auth.Domains.Subscription.Handlers;
using Fenicia.Auth.Domains.Subscription.Queries;
using Fenicia.Auth.Domains.Subscription.Responses;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Subscription;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SubscriptionController(GetUserProfileHandler getUserProfileHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves the user profile with associated companies and subscriptions.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User profile with companies and subscriptions.</returns>
    /// <response code="200">User profile retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var profile = await getUserProfileHandler.Handle(new GetUserProfileQuery(userId), ct);

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
