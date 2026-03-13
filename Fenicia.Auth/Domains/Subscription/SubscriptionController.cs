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
public class SubscriptionController(
    GetUserProfileHandler getUserProfileHandler)

    : ControllerBase
{
    [HttpGet("profile")]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile(
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        wide.UserId = userId.ToString();

        var profile = await getUserProfileHandler.Handle(new GetUserProfileQuery(userId), ct);

        return profile switch
        {
            null => NotFound(),
            _ => Ok(profile)
        };
    }
}
