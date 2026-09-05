using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.User;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<UserSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserSummaryResponse>>> SearchAsync(
        WideEventContext wide,
        [FromQuery] string? query = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var q =
            HttpContext.RequestServices
                .GetRequiredService<DefaultContext>()
                .Set<UserModel>()
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = $"%{query.Trim()}%";
            q = q.Where(u => EF.Functions.ILike(u.Name, term) || EF.Functions.ILike(u.Email, term));
        }

        var items = await q
            .OrderBy(u => u.Name)
            .Take(10)
            .Select(u => new UserSummaryResponse(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
