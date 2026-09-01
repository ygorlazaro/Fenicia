using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Fenicia.Module.Projects.Domains.Project.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    /// <summary>
    /// Gets all projects with pagination.
    /// </summary>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="query">Advanced query string for filtering. Example: <c>title[*]Alpha,status[=]Planned</c></param>
    /// <param name="sort">Sort fields. Example: <c>title,-startDate</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>A list of projects for the requested page.</returns>
    /// <response code="200">Projects retrieved successfully. Example: <c>[{ "id": "11111111-1111-1111-1111-111111111111", "title": "Project Alpha", "description": "A sample project", "status": "Planned", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222", "companyId": "33333333-3333-3333-3333-333333333333" }]</c></response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying projects.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var projects = await projectService.GetAllAsync(new GetAllProjectQuery(page, perPage, query, sort), cancellationToken);

        return Ok(projects);
    }

    /// <summary>
    /// Gets a project by its unique identifier, including its statuses and tasks.
    /// </summary>
    /// <param name="id">The unique identifier of the project. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The project details including statuses and tasks, or null if not found.</returns>
    /// <response code="200">Project found. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "title": "Project Alpha", "description": "A sample project", "status": "Planned", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222", "companyId": "33333333-3333-3333-3333-333333333333", "statuses": [], "tasks": [] }</c></response>
    /// <response code="400">Invalid ID format supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="404">Project with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying the project.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var project = await projectService.GetByIdAsync(new GetProjectByIdQuery(id), cancellationToken);

        return project is null ? NotFound() : Ok(project);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="command">The project data to create. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "title": "Project Alpha", "description": "A sample project", "status": "Planned", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created project details.</returns>
    /// <response code="201">Project created successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "title": "Project Alpha", "description": "A sample project", "status": "Planned", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222", "companyId": "33333333-3333-3333-3333-333333333333" }</c></response>
    /// <response code="400">Invalid request body or status value supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to create projects.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="ArgumentException">Thrown by the service when the Status field cannot be parsed as a valid project status.</exception>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while inserting the project.</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectResponse>> PostAsync([FromBody] AddProjectCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var project = await projectService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, project);
    }

    /// <summary>
    /// Updates an existing project by its unique identifier.
    /// </summary>
    /// <param name="command">The project data to update. Example: <c>{ "id": "00000000-0000-0000-0000-000000000000", "title": "Project Alpha Updated", "description": "An updated description", "status": "InProgress", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222" }</c></param>
    /// <param name="id">The unique identifier of the project to update. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The updated project details, or null if the project was not found.</returns>
    /// <response code="200">Project updated successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "title": "Project Alpha Updated", "description": "An updated description", "status": "InProgress", "startDate": "2024-01-15T00:00:00Z", "endDate": "2024-12-31T00:00:00Z", "owner": "22222222-2222-2222-2222-222222222222", "companyId": "33333333-3333-3333-3333-333333333333" }</c></response>
    /// <response code="400">Invalid request body or status value supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to update projects.</response>
    /// <response code="404">Project with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="ArgumentException">Thrown by the service when the Status field cannot be parsed as a valid project status.</exception>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database update.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while updating the project.</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectResponse>> PatchAsync([FromBody] UpdateProjectCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var project = await projectService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return project is null ? NotFound() : Ok(project);
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Project deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to delete projects.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database delete.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while deleting the project.</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectService.DeleteAsync(new DeleteProjectCommand(id), cancellationToken);

        return NoContent();
    }
}
