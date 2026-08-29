using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectStatusController(ProjectStatusService projectStatusService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProjectStatusResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProjectStatusResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var statuses = await projectStatusService.GetAllAsync(new GetAllProjectStatusQuery(page, perPage), ct);

        return Ok(statuses);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectStatusByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProjectStatusByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.GetByIdAsync(new GetProjectStatusByIdQuery(id), ct);

        return status is null ? NotFound() : Ok(status);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProjectStatusResponse>> PostAsync([FromBody] AddProjectStatusCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.AddAsync(command, ClaimReader.UserId(User), ct);

        return new CreatedResult(string.Empty, status);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProjectStatusResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProjectStatusResponse>> PatchAsync([FromBody] UpdateProjectStatusCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var status = await projectStatusService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

        return status is null ? NotFound() : Ok(status);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await projectStatusService.DeleteAsync(new DeleteProjectStatusCommand(id), ct);

        return NoContent();
    }
}
