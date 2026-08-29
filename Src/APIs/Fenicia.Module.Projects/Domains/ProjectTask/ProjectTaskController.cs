using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskController(ProjectTaskService projectTaskService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTasks = await projectTaskService.GetAllAsync(new GetAllProjectTaskQuery(page, perPage), ct);

        return Ok(projectTasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.GetByIdAsync(new GetProjectTaskByIdQuery(id), ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskResponse>> PostAsync([FromBody] AddProjectTaskCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.AddAsync(command, ClaimReader.UserId(User), ct);

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
    public async Task<ActionResult<UpdateProjectTaskResponse>> PatchAsync([FromBody] UpdateProjectTaskCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectTaskService.DeleteAsync(new DeleteProjectTaskCommand(id), ct);

        return NoContent();
    }
}
