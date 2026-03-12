using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Add;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Delete;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectSubtaskController(
    GetAllProjectSubtaskHandler getAllProjectSubtaskHandler,
    GetProjectSubtaskByIdHandler getProjectSubtaskByIdHandler,
    AddProjectSubtaskHandler addProjectSubtaskHandler,
    UpdateProjectSubtaskHandler updateProjectSubtaskHandler,
    DeleteProjectSubtaskHandler deleteProjectSubtaskHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectSubtaskResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectSubtaskResponse>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1, 
        [FromQuery] int perPage = 10,
        CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var projectSubtasks = await getAllProjectSubtaskHandler.Handle(new GetAllProjectSubtaskQuery(page,
                perPage),
            ct);

        return Ok(projectSubtasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectSubtaskByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectSubtaskByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var projectSubtask = await getProjectSubtaskByIdHandler.Handle(new GetProjectSubtaskByIdQuery(id),
            ct);

        return projectSubtask is null ? NotFound() : Ok(projectSubtask);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectSubtaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectSubtaskResponse>> PostAsync(
        [FromBody] AddProjectSubtaskCommand command,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var projectSubtask = await addProjectSubtaskHandler.Handle(command,
            ct);

        return new CreatedResult(string.Empty,
            projectSubtask);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectSubtaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectSubtaskResponse>> PatchAsync(
        [FromBody] UpdateProjectSubtaskCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var projectSubtask = await updateProjectSubtaskHandler.Handle(command with
            {
                Id = id
            },
            ct);

        return projectSubtask is null ? NotFound() : Ok(projectSubtask);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        await deleteProjectSubtaskHandler.Handle(new DeleteProjectSubtaskCommand(id),
            ct);

        return NoContent();
    }
}
