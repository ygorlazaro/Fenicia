using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskAssigneeController(ProjectTaskAssigneeService projectTaskAssigneeService) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskAssigneeResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskAssigneeResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignees = await projectTaskAssigneeService.GetAllAsync(new GetAllProjectTaskAssigneeQuery(page, perPage), ct);

        return Ok(assignees);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskAssigneeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskAssigneeByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(id), ct);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskAssigneeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskAssigneeResponse>> PostAsync([FromBody] AddProjectTaskAssigneeCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.AddAsync(command, ct);

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
    public async Task<ActionResult<UpdateProjectTaskAssigneeResponse>> PatchAsync([FromBody] UpdateProjectTaskAssigneeCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.UpdateAsync(command with { Id = id }, ct);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectTaskAssigneeService.DeleteAsync(new DeleteProjectTaskAssigneeCommand(id), ct);

        return NoContent();
    }
}
