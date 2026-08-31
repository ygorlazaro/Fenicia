using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

/// <summary>
/// Manages project task operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskController(ProjectTaskService projectTaskService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project tasks.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of project tasks</returns>
    /// <response code="200">List of project tasks returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project tasks</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectTaskResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectTaskResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTasks = await projectTaskService.GetAllAsync(new GetAllProjectTaskQuery(page, perPage), ct);

        return Ok(projectTasks);
    }

    /// <summary>
    /// Gets a project task by ID.
    /// </summary>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Project task data</returns>
    /// <response code="200">Project task found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project task not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project task</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTaskByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectTaskByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.GetByIdAsync(new GetProjectTaskByIdQuery(id), ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    /// <summary>
    /// Creates a new project task.
    /// </summary>
    /// <param name="command">Project task data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created project task</returns>
    /// <response code="201">Project task created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project tasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project tasks</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectTaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskResponse>> PostAsync([FromBody] AddProjectTaskCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.AddAsync(command, ClaimReader.UserId(User), ct);

        return new CreatedResult(string.Empty, projectTask);
    }

    /// <summary>
    /// Updates an existing project task.
    /// </summary>
    /// <param name="command">Updated project task data</param>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated project task</returns>
    /// <response code="200">Project task updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project tasks</response>
    /// <response code="404">Project task not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project tasks</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectTaskResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectTaskResponse>> PatchAsync([FromBody] UpdateProjectTaskCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    /// <summary>
    /// Deletes a project task.
    /// </summary>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="ct">Cancellation token</param>
    /// <response code="204">Project task deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project tasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project tasks</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectTaskService.DeleteAsync(new DeleteProjectTaskCommand(id), ct);

        return NoContent();
    }
}
