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
    public async Task<IActionResult> CreateUser([FromBody] UserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.AddAsync(request, cancellationToken);

        return Ok(user);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserByIdASync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, [FromBody] UserRequest request, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);

        if (userId != id)
        {
            ClaimReader.ValidateRole(User, "Admin");
        }

        var response = await userService.UpdateAsync(id, request, cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/follow")]
    public async Task<IActionResult> FollowAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var follower = await followerService.FollowAsync(userId, id, cancellationToken);

        if (follower is null)
        {
            return BadRequest();
        }

        return Ok(follower);
    }

    [HttpDelete("{id:guid}/unfollow")]
    public async Task<IActionResult> UnfollowAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);

        var follower = await followerService.UnfollowAsync(userId, id, cancellationToken);

        if (follower is null)
        {
            return BadRequest();
        }

        return Ok(follower);
    }
}
