using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Module.Projects.Domains.Team.DTOs;
using Fenicia.Module.Projects.Domains.Team.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Team;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TeamController(
    ITeamService teamService,
    ICompanyContext companyContext) : ControllerBase
{
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(List<GetAllTeamResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetAllTeamResponse>>> GetByProjectAsync(
        [FromRoute] Guid projectId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var teams = await teamService.GetAllByProjectAsync(projectId, cancellationToken);
        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetTeamByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetTeamByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var team = await teamService.GetByIdAsync(id, cancellationToken);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(typeof(List<TeamMemberResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeamMemberResponse>>> GetMembersAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var members = await teamService.GetMembersAsync(id, cancellationToken);
        return Ok(members);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddTeamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddTeamResponse>> PostAsync(
        [FromBody] AddTeamCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var team = await teamService.AddAsync(command, companyContext.CompanyId, cancellationToken);
        return new CreatedResult(string.Empty, team);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateTeamResponse>> PatchAsync(
        [FromBody] UpdateTeamCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        var team = await teamService.UpdateAsync(
            command with { Id = id },
            companyContext.CompanyId,
            cancellationToken);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();
        await teamService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("members")]
    [ProducesResponseType(typeof(AddTeamUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddTeamUserResponse>> AddMemberAsync(
        [FromBody] AddTeamUserCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var userId = ClaimReader.UserId(User);
        var team = await teamService.GetByIdAsync(command.TeamId, cancellationToken);
        if (team is null)
        {
            return NotFound();
        }

        if (command.UserId != userId && !await teamService.IsTeamAdminAsync(userId, command.TeamId, cancellationToken))
        {
            return Forbid();
        }

        var result = await teamService.AddMemberAsync(command, companyContext.CompanyId, cancellationToken);
        return new CreatedResult(string.Empty, result);
    }

    [HttpDelete("{teamId:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> RemoveMemberAsync(
        [FromRoute] Guid teamId,
        [FromRoute] Guid userId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var currentUser = ClaimReader.UserId(User);
        if (userId != currentUser && !await teamService.IsTeamAdminAsync(currentUser, teamId, cancellationToken))
        {
            return Forbid();
        }

        await teamService.RemoveMemberAsync(new RemoveTeamUserCommand(teamId, userId), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{teamId:guid}/members/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> UpdateMemberRoleAsync(
        [FromBody] UpdateTeamUserRoleCommand command,
        [FromRoute] Guid teamId,
        [FromRoute] Guid userId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var currentUser = ClaimReader.UserId(User);
        if (!await teamService.IsTeamAdminAsync(currentUser, teamId, cancellationToken))
        {
            return Forbid();
        }

        var ok = await teamService.UpdateMemberRoleAsync(
            command with { TeamId = teamId, UserId = userId },
            cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
