using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Follower;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.User;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController(IUserService userService, IFollowerService followerService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateUser([FromBody] UserRequest request, CancellationToken ct)
    {
        var user = await userService.AddAsync(request, ct);

        return Ok(user);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetUserByIdASync([FromRoute] Guid id, CancellationToken ct)
    {
        var user = await userService.GetByIdAsync(id, ct);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> UpdateUserAsync(
        [FromRoute] Guid id,
        [FromBody] UserRequest request,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);

        if (userId != id)
        {
            ClaimReader.ValidateRole(this.User, "Admin");
        }

        var response = await userService.UpdateAsync(id, request, ct);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid id, CancellationToken ct)
    {
        await userService.DeleteAsync(id, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/follow")]
    public async Task<ActionResult> FollowAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var follower = await followerService.FollowAsync(userId, id, ct);

        return follower is null ? BadRequest() : Ok(follower);
    }

    [HttpDelete("{id:guid}/unfollow")]
    public async Task<ActionResult> UnfollowAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);

        var follower = await followerService.UnfollowAsync(userId, id, ct);

        return follower is null ? BadRequest() : Ok(follower);
    }
}