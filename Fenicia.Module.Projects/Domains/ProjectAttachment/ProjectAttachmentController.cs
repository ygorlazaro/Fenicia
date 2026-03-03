using System.Net.Mime;

using Fenicia.Module.Projects.Domains.ProjectAttachment.Add;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Delete;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetById;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectAttachmentController(
    GetAllProjectAttachmentHandler getAllProjectAttachmentHandler,
    GetProjectAttachmentByIdHandler getProjectAttachmentByIdHandler,
    AddProjectAttachmentHandler addProjectAttachmentHandler,
    UpdateProjectAttachmentHandler updateProjectAttachmentHandler,
    DeleteProjectAttachmentHandler deleteProjectAttachmentHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectAttachmentResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectAttachmentResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var projectAttachments = await getAllProjectAttachmentHandler.Handle(new GetAllProjectAttachmentQuery(page, perPage), ct);

        return Ok(projectAttachments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectAttachmentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectAttachmentByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var projectAttachment = await getProjectAttachmentByIdHandler.Handle(new GetProjectAttachmentByIdQuery(id), ct);

        return projectAttachment is null ? NotFound() : Ok(projectAttachment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectAttachmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectAttachmentResponse>> PostAsync([FromBody] AddProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = await addProjectAttachmentHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, projectAttachment);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectAttachmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectAttachmentResponse>> PatchAsync(
        [FromBody] UpdateProjectAttachmentCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var projectAttachment = await updateProjectAttachmentHandler.Handle(command with { Id = id }, ct);

        return projectAttachment is null ? NotFound() : Ok(projectAttachment);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectAttachmentHandler.Handle(new DeleteProjectAttachmentCommand(id), ct);

        return NoContent();
    }
}
