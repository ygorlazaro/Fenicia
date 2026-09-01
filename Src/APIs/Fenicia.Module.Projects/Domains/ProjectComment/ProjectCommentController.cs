using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectComment;

/// <summary>
/// Manages project comment operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectCommentController(IProjectCommentService projectCommentService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project comments.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="query">Advanced query string for filtering. Example: <c>content[*]alpha</c></param>
    /// <param name="sort">Sort fields. Example: <c>-createdAt</c></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of project comments</returns>
    /// <response code="200">List of project comments returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project comments</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectCommentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectCommentResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectComments = await projectCommentService.GetAllAsync(new GetAllProjectCommentQuery(page, perPage, query, sort), cancellationToken);

        return Ok(projectComments);
    }

    /// <summary>
    /// Gets a project comment by ID.
    /// </summary>
    /// <param name="id">Project comment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project comment data</returns>
    /// <response code="200">Project comment found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project comment not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project comment</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectCommentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectCommentByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectComment = await projectCommentService.GetByIdAsync(new GetProjectCommentByIdQuery(id), cancellationToken);

        return projectComment is null ? NotFound() : Ok(projectComment);
    }

    /// <summary>
    /// Creates a new project comment.
    /// </summary>
    /// <param name="command">Project comment data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project comment</returns>
    /// <response code="201">Project comment created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project comments</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project comments</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectCommentResponse>> PostAsync([FromBody] AddProjectCommentCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectComment = await projectCommentService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, projectComment);
    }

    /// <summary>
    /// Updates an existing project comment.
    /// </summary>
    /// <param name="command">Updated project comment data</param>
    /// <param name="id">Project comment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project comment</returns>
    /// <response code="200">Project comment updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project comments</response>
    /// <response code="404">Project comment not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project comments</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectCommentResponse>> PatchAsync([FromBody] UpdateProjectCommentCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectComment = await projectCommentService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return projectComment is null ? NotFound() : Ok(projectComment);
    }

    /// <summary>
    /// Deletes a project comment.
    /// </summary>
    /// <param name="id">Project comment ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Project comment deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project comments</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project comments</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectCommentService.DeleteAsync(new DeleteProjectCommentCommand(id), cancellationToken);

        return NoContent();
    }
}
