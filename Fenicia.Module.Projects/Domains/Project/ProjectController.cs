using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Project;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Project;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> PostAsync([FromBody] ProjectRequest request, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var project = await projectService.AddAsync(request, userId, ct);

        return project is null ? new BadRequestObjectResult("Project not found") : new CreatedResult("", project);
    }
}