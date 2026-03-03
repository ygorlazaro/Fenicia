using System.Net.Mime;

using Fenicia.Module.Projects.Domains.ProjectComment.Add;
using Fenicia.Module.Projects.Domains.ProjectComment.Delete;
using Fenicia.Module.Projects.Domains.ProjectComment.GetAll;
using Fenicia.Module.Projects.Domains.ProjectComment.GetById;
using Fenicia.Module.Projects.Domains.ProjectComment.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectCommentController(
    GetAllProjectCommentHandler getAllProjectCommentHandler,
    GetProjectCommentByIdHandler getProjectCommentByIdHandler,
    AddProjectCommentHandler addProjectCommentHandler,
    UpdateProjectCommentHandler updateProjectCommentHandler,
    DeleteProjectCommentHandler deleteProjectCommentHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectCommentResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectCommentResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var projectComments = await getAllProjectCommentHandler.Handle(new GetAllProjectCommentQuery(page, perPage), ct);

        return Ok(projectComments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectCommentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectCommentByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var projectComment = await getProjectCommentByIdHandler.Handle(new GetProjectCommentByIdQuery(id), ct);

        return projectComment is null ? NotFound() : Ok(projectComment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectCommentResponse>> PostAsync([FromBody] AddProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = await addProjectCommentHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, projectComment);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectCommentResponse>> PatchAsync(
        [FromBody] UpdateProjectCommentCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var projectComment = await updateProjectCommentHandler.Handle(command with { Id = id }, ct);

        return projectComment is null ? NotFound() : Ok(projectComment);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProjectCommentHandler.Handle(new DeleteProjectCommentCommand(id), ct);

        return NoContent();
    }
}
