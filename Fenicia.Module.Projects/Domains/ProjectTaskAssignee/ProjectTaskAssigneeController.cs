using System.Net.Mime;

using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Delete;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskAssigneeController(
    GetAllProjectTaskAssigneeHandler getAllProjectTaskAssigneeHandler,
    GetProjectTaskAssigneeByIdHandler getProjectTaskAssigneeByIdHandler,
    AddProjectTaskAssigneeHandler addProjectTaskAssigneeHandler,
    UpdateProjectTaskAssigneeHandler updateProjectTaskAssigneeHandler,
    DeleteProjectTaskAssigneeHandler deleteProjectTaskAssigneeHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskAssigneeResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskAssigneeResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var assignees = await getAllProjectTaskAssigneeHandler.Handle(new GetAllProjectTaskAssigneeQuery(page, perPage), ct);

        return Ok(assignees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskAssigneeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskAssigneeByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var assignee = await getProjectTaskAssigneeByIdHandler.Handle(new GetProjectTaskAssigneeByIdQuery(id), ct);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskAssigneeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskAssigneeResponse>> PostAsync([FromBody] AddProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = await addProjectTaskAssigneeHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, assignee);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectTaskAssigneeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectTaskAssigneeResponse>> PatchAsync(
        [FromBody] UpdateProjectTaskAssigneeCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var assignee = await updateProjectTaskAssigneeHandler.Handle(command with { Id = id }, ct);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectTaskAssigneeHandler.Handle(new DeleteProjectTaskAssigneeCommand(id), ct);

        return NoContent();
    }
}
