using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

/// <summary>
/// Manages project status operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectStatusController(ProjectStatusService projectStatusService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of project statuses.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="query">Advanced query string for filtering. Example: <c>name[=]Active</c></param>
    /// <param name="sort">Sort fields. Example: <c>order</c></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of project statuses</returns>
    /// <response code="200">List of project statuses returned successfully</response>
    /// <response code="400">Invalid pagination parameters</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access project statuses</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectStatusResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectStatusResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var statuses = await projectStatusService.GetAllAsync(new GetAllProjectStatusQuery(page, perPage, query, sort), cancellationToken);

        return Ok(statuses);
    }

    /// <summary>
    /// Gets a project status by ID.
    /// </summary>
    /// <param name="id">Project status ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project status data</returns>
    /// <response code="200">Project status found</response>
    /// <response code="400">Invalid ID</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Project status not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to access the project status</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectStatusByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectStatusByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.GetByIdAsync(new GetProjectStatusByIdQuery(id), cancellationToken);

        return status is null ? NotFound() : Ok(status);
    }

    /// <summary>
    /// Creates a new project status.
    /// </summary>
    /// <param name="command">Project status data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project status</returns>
    /// <response code="201">Project status created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create project statuses</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create project statuses</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectStatusResponse>> PostAsync([FromBody] AddProjectStatusCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, status);
    }

    /// <summary>
    /// Updates an existing project status.
    /// </summary>
    /// <param name="command">Updated project status data</param>
    /// <param name="id">Project status ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project status</returns>
    /// <response code="200">Project status updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update project statuses</response>
    /// <response code="404">Project status not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update project statuses</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectStatusResponse>> PatchAsync([FromBody] UpdateProjectStatusCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return status is null ? NotFound() : Ok(status);
    }

    /// <summary>
    /// Deletes a project status.
    /// </summary>
    /// <param name="id">Project status ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Project status deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete project statuses</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete project statuses</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectStatusService.DeleteAsync(new DeleteProjectStatusCommand(id), cancellationToken);

        return NoContent();
    }
}
