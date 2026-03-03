using System.Net.Mime;

using Fenicia.Module.Projects.Domains.Project.Add;
using Fenicia.Module.Projects.Domains.Project.Delete;
using Fenicia.Module.Projects.Domains.Project.GetAll;
using Fenicia.Module.Projects.Domains.Project.GetById;
using Fenicia.Module.Projects.Domains.Project.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Project;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectController(
    GetAllProjectHandler getAllProjectHandler,
    GetProjectByIdHandler getProductByIdHandler,
    AddProjectHandler addProjectHandler,
    UpdateProjectHandler updateProjectHandler,
    DeleteProjectHandler deleteProjectHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var projects = await getAllProjectHandler.Handle(new GetAllProjectQuery(page, perPage), ct);

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var project = await getProductByIdHandler.Handle(new GetProjectByIdQuery(id), ct);

        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectResponse>> PostAsync([FromBody] AddProjectCommand command, CancellationToken ct)
    {
        var project = await addProjectHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, project);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectResponse>> PatchAsync(
        [FromBody] UpdateProjectCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var project = await updateProjectHandler.Handle(command with { Id = id }, ct);

        return project is null ? NotFound() : Ok(project);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectHandler.Handle(new DeleteProjectCommand(id), ct);

        return NoContent();
    }
}
