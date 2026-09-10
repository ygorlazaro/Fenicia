using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedTeams(DefaultContext context)
    {
        if (context.Teams.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var projects = context.Projects.ToList();
        var users = context.AuthUsers.ToList();

        if (!projects.Any() || !users.Any())
        {
            return;
        }

        var teamNames = new[]
        {
            "Frontend", "Backend", "Mobile", "QA", "DevOps"
        };

        var teamColors = new[]
        {
            "#0d6efd", "#198754", "#ffc107", "#dc3545", "#6f42c1"
        };

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 20; i++)
        {
            var teamIndex = i % 5;

            var team = new TeamModel
            {
                CompanyId = companyId,
                ProjectId = projects[(i - 1) % projects.Count].Id,
                Name = $"Time {i} — {teamNames[teamIndex]}",
                Description = $"Time responsável pela squad {i}",
                Color = teamColors[teamIndex],
                CreatedBy = users[(i - 1) % users.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.Teams.Add(team);
        }
    }

    private static void SeedTeamUsers(DefaultContext context)
    {
        if (context.ProjectTeamUsers.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var teams = context.Teams.ToList();
        var users = context.AuthUsers.ToList();

        if (!teams.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 60; i++)
        {
            var joinedAt = now.AddDays(-(random.NextDouble() * 365));

            var teamUser = new TeamUserModel
            {
                CompanyId = companyId,
                TeamId = teams[(i - 1) % teams.Count].Id,
                UserId = users[(i - 1) % users.Count].Id,
                Role = (EnumTeamRole)(i % 2),
                JoinedAt = joinedAt,
                Created = now.AddDays(-random.NextDouble() * 30),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectTeamUsers.Add(teamUser);
        }
    }
}
