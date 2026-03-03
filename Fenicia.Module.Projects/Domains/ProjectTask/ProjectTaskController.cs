using System.Net.Mime;

using Fenicia.Module.Projects.Domains.ProjectTask.Add;
using Fenicia.Module.Projects.Domains.ProjectTask.Delete;
using Fenicia.Module.Projects.Domains.ProjectTask.GetAll;
using Fenicia.Module.Projects.Domains.ProjectTask.GetById;
using Fenicia.Module.Projects.Domains.ProjectTask.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskController(
    GetAllProjectTaskHandler getAllProjectTaskHandler,
    GetProjectTaskByIdHandler getProjectTaskByIdHandler,
    AddProjectTaskHandler addProjectTaskHandler,
    UpdateProjectTaskHandler updateProjectTaskHandler,
    DeleteProjectTaskHandler deleteProjectTaskHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var projectTasks = await getAllProjectTaskHandler.Handle(new GetAllProjectTaskQuery(page, perPage), ct);

        return Ok(projectTasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var projectTask = await getProjectTaskByIdHandler.Handle(new GetProjectTaskByIdQuery(id), ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskResponse>> PostAsync([FromBody] AddProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = await addProjectTaskHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, projectTask);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectTaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectTaskResponse>> PatchAsync(
        [FromBody] UpdateProjectTaskCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var projectTask = await updateProjectTaskHandler.Handle(command with { Id = id }, ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectTaskHandler.Handle(new DeleteProjectTaskCommand(id), ct);

        return NoContent();
    }
}
