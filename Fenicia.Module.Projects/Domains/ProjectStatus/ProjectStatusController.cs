using System.Net.Mime;

using Fenicia.Module.Projects.Domains.ProjectStatus.Add;
using Fenicia.Module.Projects.Domains.ProjectStatus.Delete;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetAll;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetById;
using Fenicia.Module.Projects.Domains.ProjectStatus.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectStatusController(
    GetAllProjectStatusHandler getAllProjectStatusHandler,
    GetProjectStatusByIdHandler getProjectStatusByIdHandler,
    AddProjectStatusHandler addProjectStatusHandler,
    UpdateProjectStatusHandler updateProjectStatusHandler,
    DeleteProjectStatusHandler deleteProjectStatusHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectStatusResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectStatusResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var statuses = await getAllProjectStatusHandler.Handle(new GetAllProjectStatusQuery(page, perPage), ct);

        return Ok(statuses);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectStatusByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectStatusByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var status = await getProjectStatusByIdHandler.Handle(new GetProjectStatusByIdQuery(id), ct);

        return status is null ? NotFound() : Ok(status);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectStatusResponse>> PostAsync([FromBody] AddProjectStatusCommand command, CancellationToken ct)
    {
        var status = await addProjectStatusHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, status);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectStatusResponse>> PatchAsync(
        [FromBody] UpdateProjectStatusCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var status = await updateProjectStatusHandler.Handle(command with { Id = id }, ct);

        return status is null ? NotFound() : Ok(status);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectStatusHandler.Handle(new DeleteProjectStatusCommand(id), ct);

        return NoContent();
    }
}
