using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

/// <summary>
/// Manages project attachment operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectAttachmentController(ProjectAttachmentService projectAttachmentService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project attachments.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of project attachments</returns>
    /// <response code="200">List of project attachments returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project attachments</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectAttachmentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectAttachmentResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectAttachments = await projectAttachmentService.GetAllAsync(new GetAllProjectAttachmentQuery(page, perPage), ct);

        return Ok(projectAttachments);
    }

    /// <summary>
    /// Gets a project attachment by ID.
    /// </summary>
    /// <param name="id">Project attachment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Project attachment data</returns>
    /// <response code="200">Project attachment found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project attachment not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project attachment</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectAttachmentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectAttachmentByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectAttachment = await projectAttachmentService.GetByIdAsync(new GetProjectAttachmentByIdQuery(id), ct);

        return projectAttachment is null ? NotFound() : Ok(projectAttachment);
    }

    /// <summary>
    /// Creates a new project attachment.
    /// </summary>
    /// <param name="command">Project attachment data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created project attachment</returns>
    /// <response code="201">Project attachment created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project attachments</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project attachments</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectAttachmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectAttachmentResponse>> PostAsync([FromBody] AddProjectAttachmentCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectAttachment = await projectAttachmentService.AddAsync(command, ClaimReader.UserId(User), ct);

        return new CreatedResult(string.Empty, projectAttachment);
    }

    /// <summary>
    /// Updates an existing project attachment.
    /// </summary>
    /// <param name="command">Updated project attachment data</param>
    /// <param name="id">Project attachment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated project attachment</returns>
    /// <response code="200">Project attachment updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project attachments</response>
    /// <response code="404">Project attachment not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project attachments</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectAttachmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectAttachmentResponse>> PatchAsync([FromBody] UpdateProjectAttachmentCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectAttachment = await projectAttachmentService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

        return projectAttachment is null ? NotFound() : Ok(projectAttachment);
    }

    /// <summary>
    /// Deletes a project attachment.
    /// </summary>
    /// <param name="id">Project attachment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <response code="204">Project attachment deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project attachments</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project attachments</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectAttachmentService.DeleteAsync(new DeleteProjectAttachmentCommand(id), ct);

        return NoContent();
    }
}
