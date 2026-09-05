using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectTask;

/// <inheritdoc />
/// <summary>
///     Manages project task operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectTaskController(
    IProjectTaskService projectTaskService,
    ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    ///     Gets a paginated list of project tasks.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="query">Advanced query string for filtering. Example: <c>title[*]Alpha</c></param>
    /// <param name="sort">Sort fields. Example: <c>title,-dueDate</c></param>
    /// <param name="cancellationToken">Cancellation token</param>
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
    public async Task<ActionResult<List<GetAllProjectTaskResponse>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTasks = await projectTaskService.GetAllAsync(
            new GetAllProjectTaskQuery(page, perPage, query, sort),
            cancellationToken);

        return Ok(projectTasks);
    }

    /// <summary>
    ///     Gets a project task by ID.
    /// </summary>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
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
    public async Task<ActionResult<GetProjectTaskByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.GetByIdAsync(new GetProjectTaskByIdQuery(id), cancellationToken);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    /// <summary>
    ///     Creates a new project task.
    /// </summary>
    /// <param name="command">Project task data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project task</returns>
    /// <response code="201">Project task created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project tasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project tasks</exception>
    [HttpPost]
    [ProducesResponseType(typeof(AddProjectTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectTaskResponse>> PostAsync(
        [FromBody] AddProjectTaskCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.AddAsync(command, companyContext.CompanyId, cancellationToken);

        return new CreatedResult(string.Empty, projectTask);
    }

    /// <summary>
    ///     Updates an existing project task.
    /// </summary>
    /// <param name="command">Updated project task data</param>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project task</returns>
    /// <response code="200">Project task updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project tasks</response>
    /// <response code="404">Project task not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project tasks</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UpdateProjectTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectTaskResponse>> PatchAsync(
        [FromBody] UpdateProjectTaskCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projectTask = await projectTaskService.UpdateAsync(
            command with { Id = id },
            companyContext.CompanyId,
            cancellationToken);

        return projectTask is null ? NotFound() : Ok(projectTask);
    }

    /// <summary>
    ///     Deletes a project task.
    /// </summary>
    /// <param name="id">Project task ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Project task deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project tasks</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project tasks</exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectTaskService.DeleteAsync(new DeleteProjectTaskCommand(id), cancellationToken);

        return NoContent();
    }
}