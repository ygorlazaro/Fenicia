using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

/// <summary>
/// Manages project task assignee operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskAssigneeController(ProjectTaskAssigneeService projectTaskAssigneeService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project task assignees.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="query">Advanced query string for filtering. Example: <c>userId[=]11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="sort">Sort fields. Example: <c>assignedAt</c></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of project task assignees</returns>
    /// <response code="200">List of project task assignees returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project task assignees</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskAssigneeResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskAssigneeResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignees = await projectTaskAssigneeService.GetAllAsync(new GetAllProjectTaskAssigneeQuery(page, perPage, query, sort), cancellationToken);

        return Ok(assignees);
    }

    /// <summary>
    /// Gets a project task assignee by ID.
    /// </summary>
    /// <param name="id">Project task assignee ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project task assignee data</returns>
    /// <response code="200">Project task assignee found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project task assignee not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project task assignee</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskAssigneeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskAssigneeByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(id), cancellationToken);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    /// <summary>
    /// Creates a new project task assignee.
    /// </summary>
    /// <param name="command">Project task assignee data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project task assignee</returns>
    /// <response code="201">Project task assignee created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project task assignees</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project task assignees</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskAssigneeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskAssigneeResponse>> PostAsync([FromBody] AddProjectTaskAssigneeCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, assignee);
    }

    /// <summary>
    /// Updates an existing project task assignee.
    /// </summary>
    /// <param name="command">Updated project task assignee data</param>
    /// <param name="id">Project task assignee ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project task assignee</returns>
    /// <response code="200">Project task assignee updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project task assignees</response>
    /// <response code="404">Project task assignee not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project task assignees</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectTaskAssigneeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectTaskAssigneeResponse>> PatchAsync([FromBody] UpdateProjectTaskAssigneeCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var assignee = await projectTaskAssigneeService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return assignee is null ? NotFound() : Ok(assignee);
    }

    /// <summary>
    /// Deletes a project task assignee.
    /// </summary>
    /// <param name="id">Project task assignee ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Project task assignee deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project task assignees</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project task assignees</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectTaskAssigneeService.DeleteAsync(new DeleteProjectTaskAssigneeCommand(id), cancellationToken);

        return NoContent();
    }
}
