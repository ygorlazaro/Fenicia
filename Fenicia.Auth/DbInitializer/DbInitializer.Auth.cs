using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedRoles(DefaultContext context)
    {
        if (!context.AuthRoles.Any())
        {
            context.AuthRoles.AddRange(
                new RoleModel { Name = "God" },
                new RoleModel { Name = "Admin" },
                new RoleModel { Name = "User" });
        }
    }

    private static void SeedUser(DefaultContext context)
    {
        if (context.AuthUsers.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var godRole = context.AuthRoles.FirstOrDefault(x => x.Name == "God");
        var userRole = context.AuthRoles.FirstOrDefault(x => x.Name == "User");

        for (var i = 1; i <= 100; i++)
        {
            var user = new UserModel
            {
                Email = i == 1 ? "ygor@ygorlazaro.com" : $"usuario{i}@fenicia.com",
                Name = i == 1 ? "Ygor Lazaro" : $"Usuario {i}",
                Password = SecurityService.Hash("ygor")
            };

            var role = new UserRoleModel
            {
                User = user,
                CompanyId = companyId,
                RoleId = i == 1 ? godRole?.Id ?? Guid.Empty : userRole?.Id ?? Guid.Empty
            };

            context.AuthUsers.Add(user);
            context.AuthUserRoles.Add(role);
        }

        context.SaveChanges();
    }

    private static void SeedForgottenPasswords(DefaultContext context)
    {
        if (context.AuthForgottenPasswords.Any())
        {
            return;
        }

        var users = context.AuthUsers.ToList();

        if (!users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 20; i++)
        {
            var expirationDate = now.AddDays(random.NextDouble() * 7);

            var forgotPassword = new ForgotPasswordModel
            {
                UserId = users[(i - 1) % users.Count].Id,
                Code = $"code-{i}",
                ExpirationDate = expirationDate,
                IsActive = i % 2 == 0,
                IpAddress = $"192.168.{(i % 255) + 1}.{(i % 255) + 1}",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                Created = now.AddDays(-random.NextDouble() * 7),
                Updated = now.AddDays(-random.NextDouble() * 7)
            };

            context.AuthForgottenPasswords.Add(forgotPassword);
        }
    }
}
