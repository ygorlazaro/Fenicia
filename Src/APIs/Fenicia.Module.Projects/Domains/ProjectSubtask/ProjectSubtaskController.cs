using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

/// <summary>
/// Manages project subtask operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectSubtaskController(ProjectSubtaskService projectSubtaskService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project subtasks.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of project subtasks</returns>
    /// <response code="200">List of project subtasks returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project subtasks</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectSubtaskResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectSubtaskResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectSubtasks = await projectSubtaskService.GetAllAsync(new GetAllProjectSubtaskQuery(page, perPage), ct);

        return Ok(projectSubtasks);
    }

    /// <summary>
    /// Gets a project subtask by ID.
    /// </summary>
    /// <param name="id">Project subtask ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Project subtask data</returns>
    /// <response code="200">Project subtask found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project subtask not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project subtask</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectSubtaskByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectSubtaskByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectSubtask = await projectSubtaskService.GetByIdAsync(new GetProjectSubtaskByIdQuery(id), ct);

        return projectSubtask is null ? NotFound() : Ok(projectSubtask);
    }

    /// <summary>
    /// Creates a new project subtask.
    /// </summary>
    /// <param name="command">Project subtask data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created project subtask</returns>
    /// <response code="201">Project subtask created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project subtasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project subtasks</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectSubtaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectSubtaskResponse>> PostAsync([FromBody] AddProjectSubtaskCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectSubtask = await projectSubtaskService.AddAsync(command, ClaimReader.UserId(User), ct);

        return new CreatedResult(string.Empty, projectSubtask);
    }

    /// <summary>
    /// Updates an existing project subtask.
    /// </summary>
    /// <param name="command">Updated project subtask data</param>
    /// <param name="id">Project subtask ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated project subtask</returns>
    /// <response code="200">Project subtask updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project subtasks</response>
    /// <response code="404">Project subtask not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project subtasks</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectSubtaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectSubtaskResponse>> PatchAsync([FromBody] UpdateProjectSubtaskCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectSubtask = await projectSubtaskService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

        return projectSubtask is null ? NotFound() : Ok(projectSubtask);
    }

    /// <summary>
    /// Deletes a project subtask.
    /// </summary>
    /// <param name="id">Project subtask ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <response code="204">Project subtask deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project subtasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project subtasks</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectSubtaskService.DeleteAsync(new DeleteProjectSubtaskCommand(id), ct);

        return NoContent();
    }
}
