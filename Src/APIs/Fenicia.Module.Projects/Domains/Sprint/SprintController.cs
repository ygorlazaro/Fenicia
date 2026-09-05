using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Module.Projects.Domains.Sprint.DTOs;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Sprint;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SprintController(ISprintService sprintService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    ///     Gets a paginated list of sprints.
    /// </summary>
    /// <param name="wide">Wide event context</param>
    /// <param name="projectId">Filter by project ID</param>
    /// <param name="page">Page number</param>
    /// <param name="perPage">Items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sprints</returns>
    /// <response code="200">List of sprints returned successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllSprintResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllSprintResponse>>> GetAsync(
        WideEventContext wide,
        [FromQuery] Guid? projectId = null,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var sprints = await sprintService.GetAllAsync(new GetAllSprintQuery(page, perPage, projectId), cancellationToken);

        return Ok(sprints);
    }

    /// <summary>
    ///     Gets a sprint by ID.
    /// </summary>
    /// <param name="id">Sprint ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sprint data</returns>
    /// <response code="200">Sprint found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Sprint not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSprintByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSprintByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var sprint = await sprintService.GetByIdAsync(new GetSprintByIdQuery(id), cancellationToken);

        return sprint is null ? NotFound() : Ok(sprint);
    }

    /// <summary>
    ///     Creates a new sprint.
    /// </summary>
    /// <param name="command">Sprint data</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sprint</returns>
    /// <response code="201">Sprint created successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to create sprints</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to create sprints</exception>
    [HttpPost]
    [ProducesResponseType(typeof(AddSprintResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddSprintResponse>> PostAsync(
        [FromBody] AddSprintCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var sprint = await sprintService.AddAsync(command, companyContext.CompanyId, cancellationToken);

        return new CreatedResult(string.Empty, sprint);
    }

    /// <summary>
    ///     Updates an existing sprint.
    /// </summary>
    /// <param name="command">Updated sprint data</param>
    /// <param name="id">Sprint ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated sprint</returns>
    /// <response code="200">Sprint updated successfully</response>
    /// <response code="400">Invalid payload</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update sprints</response>
    /// <response code="404">Sprint not found</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to update sprints</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UpdateSprintResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateSprintResponse>> PatchAsync(
        [FromBody] UpdateSprintCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var sprint = await sprintService.UpdateAsync(command with { Id = id }, companyContext.CompanyId, cancellationToken);

        return sprint is null ? NotFound() : Ok(sprint);
    }

    /// <summary>
    ///     Deletes a sprint.
    /// </summary>
    /// <param name="id">Sprint ID</param>
    /// <param name="wide">Wide event context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Sprint deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete sprints</response>
    /// <response code="500">Internal server error</response>
    /// <exception cref="UnauthorizedAccessException">User not authorized to delete sprints</exception>
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

        await sprintService.DeleteAsync(new DeleteSprintCommand(id), cancellationToken);

        return NoContent();
    }
}
