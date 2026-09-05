using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.Team.DTOs;
using Fenicia.Module.Projects.Domains.Team.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Team;

public class TeamService(
    ITeamRepository teamRepository,
    ITeamUserRepository teamUserRepository,
    IRepository<ProjectModel> projectRepository) : ITeamService
{
    public async Task<List<GetAllTeamResponse>> GetAllByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var teams = await teamRepository.Query()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        if (teams.Count == 0)
        {
            return [];
        }

        var teamIds = teams.Select(t => t.Id).ToList();
        var members = await teamUserRepository.Query()
            .Where(tu => teamIds.Contains(tu.TeamId))
            .ToListAsync(cancellationToken);

        var membersByTeam = members
            .GroupBy(m => m.TeamId)
            .ToDictionary(g => g.Key, g => g.OrderBy(m => m.JoinedAt).ToList());

        return
        [
            .. teams.Select(t => new GetAllTeamResponse(
                t.Id,
                t.ProjectId,
                t.Name,
                t.Description,
                t.Color,
                t.CreatedBy,
                t.CompanyId,
                membersByTeam.GetValueOrDefault(t.Id)?.Count ?? 0,
                [.. (membersByTeam.GetValueOrDefault(t.Id) ?? []).Select(m => new TeamMemberResponse(
                    m.UserId,
                    m.User.Name,
                    m.User.Email,
                    m.Role.ToString(),
                    m.JoinedAt))]))
        ];
    }

    public async Task<GetTeamByIdResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var team = await teamRepository.Query()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team is null)
        {
            return null;
        }

        var members = await teamUserRepository.GetByTeamAsync(id, cancellationToken);

        return new GetTeamByIdResponse(
            team.Id,
            team.ProjectId,
            team.Name,
            team.Description,
            team.Color,
            team.CreatedBy,
            team.CompanyId,
            [.. members.Select(m => new TeamMemberResponse(
                m.UserId,
                m.User.Name,
                m.User.Email,
                m.Role.ToString(),
                m.JoinedAt))]);
    }

    public async Task<AddTeamResponse> AddAsync(
        AddTeamCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var model = new TeamModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            Name = command.Name,
            Description = command.Description,
            Color = string.IsNullOrWhiteSpace(command.Color) ? "#6366f1" : command.Color,
            CreatedBy = command.CreatedBy,
            CompanyId = companyId,
        };

        var created = await teamRepository.InsertAsync(model, cancellationToken);
        return new AddTeamResponse(
            created.Id,
            created.ProjectId,
            created.Name,
            created.Description,
            created.Color,
            created.CreatedBy,
            created.CompanyId);
    }

    public async Task<UpdateTeamResponse?> UpdateAsync(
        UpdateTeamCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var team = await teamRepository.GetByIdAsync(command.Id, cancellationToken);
        if (team is null)
        {
            return null;
        }

        team.Name = command.Name;
        team.Description = command.Description;
        team.Color = string.IsNullOrWhiteSpace(command.Color) ? "#6366f1" : command.Color;
        team.CompanyId = companyId;

        await teamRepository.UpdateAsync(command.Id, team, cancellationToken);
        return new UpdateTeamResponse(
            team.Id,
            team.ProjectId,
            team.Name,
            team.Description,
            team.Color,
            team.CreatedBy,
            team.CompanyId);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var members = await teamUserRepository.GetByTeamAsync(id, cancellationToken);
        foreach (var m in members)
        {
            await teamUserRepository.DeleteAsync(m.Id, cancellationToken);
        }

        await teamRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<AddTeamUserResponse> AddMemberAsync(
        AddTeamUserCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var existing = await teamUserRepository.GetByTeamAndUserAsync(command.TeamId, command.UserId, cancellationToken);
        if (existing is not null)
        {
            return new AddTeamUserResponse(
                existing.Id,
                existing.TeamId,
                existing.UserId,
                existing.Role.ToString(),
                existing.JoinedAt,
                existing.CompanyId);
        }

        var role = Enum.Parse<EnumTeamRole>(command.Role, true);
        var model = new TeamUserModel
        {
            Id = command.Id,
            TeamId = command.TeamId,
            UserId = command.UserId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
            CompanyId = companyId,
        };

        var created = await teamUserRepository.InsertAsync(model, cancellationToken);
        return new AddTeamUserResponse(
            created.Id,
            created.TeamId,
            created.UserId,
            created.Role.ToString(),
            created.JoinedAt,
            created.CompanyId);
    }

    public async Task RemoveMemberAsync(
        RemoveTeamUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await teamUserRepository.GetByTeamAndUserAsync(command.TeamId, command.UserId, cancellationToken);
        if (existing is not null)
        {
            await teamUserRepository.DeleteAsync(existing.Id, cancellationToken);
        }
    }

    public async Task<bool> UpdateMemberRoleAsync(
        UpdateTeamUserRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var existing = await teamUserRepository.GetByTeamAndUserAsync(command.TeamId, command.UserId, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        var role = Enum.Parse<EnumTeamRole>(command.Role, true);
        existing.Role = role;
        await teamUserRepository.UpdateAsync(existing.Id, existing, cancellationToken);
        return true;
    }

    public async Task<bool> IsTeamAdminAsync(
        Guid userId,
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var membership = await teamUserRepository.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
        return membership is not null && membership.Role == EnumTeamRole.Admin;
    }

    public async Task<bool> IsTeamMemberAsync(
        Guid userId,
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var membership = await teamUserRepository.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
        return membership is not null;
    }

    public async Task<List<TeamMemberResponse>> GetMembersAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var members = await teamUserRepository.GetByTeamAsync(teamId, cancellationToken);
        return
        [
            .. members.Select(m => new TeamMemberResponse(
                m.UserId,
                m.User.Name,
                m.User.Email,
                m.Role.ToString(),
                m.JoinedAt))
        ];
    }

    public async Task<bool> HasAnyAdminInProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var teamIds = await teamRepository.Query()
            .Where(t => t.ProjectId == projectId)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        if (teamIds.Count == 0)
        {
            return false;
        }

        return await teamUserRepository.Query()
            .AnyAsync(tu => teamIds.Contains(tu.TeamId) && tu.Role == EnumTeamRole.Admin, cancellationToken);
    }

    public async Task<bool> IsProjectAdminAsync(
        Guid userId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project is not null && project.Owner == userId)
        {
            return true;
        }

        var teamIds = await teamRepository.Query()
            .Where(t => t.ProjectId == projectId)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        if (teamIds.Count == 0)
        {
            return false;
        }

        return await teamUserRepository.Query()
            .AnyAsync(tu => teamIds.Contains(tu.TeamId) && tu.UserId == userId && tu.Role == EnumTeamRole.Admin, cancellationToken);
    }
}
