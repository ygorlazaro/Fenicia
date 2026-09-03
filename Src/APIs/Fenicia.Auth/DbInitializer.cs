using System.Linq;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fenicia.Auth;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
            Seed(context);
            return;
        }

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
            Seed(context);
        }
    }

    private static void Seed(DefaultContext context)
    {
        SeedRoles(context);
        SeedStates(context);
        SeedModules(context);
        context.SaveChanges();
    }

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

    private static void SeedStates(DefaultContext context)
    {
        if (!context.AuthStates.Any())
        {
            var states = new List<StateModel>
            {
                new() { Name = "Acre", Uf = "AC" },
                new() { Name = "Alagoas", Uf = "AL" },
                new() { Name = "Amapá", Uf = "AP" },
                new() { Name = "Amazonas", Uf = "AM" },
                new() { Name = "Bahia", Uf = "BA" },
                new() { Name = "Ceará", Uf = "CE" },
                new() { Name = "Distrito Federal", Uf = "DF" },
                new() { Name = "Espírito Santo", Uf = "ES" },
                new() { Name = "Goiás", Uf = "GO" },
                new() { Name = "Maranhão", Uf = "MA" },
                new() { Name = "Mato Grosso", Uf = "MT" },
                new() { Name = "Mato Grosso do Sul", Uf = "MS" },
                new() { Name = "Minas Gerais", Uf = "MG" },
                new() { Name = "Pará", Uf = "PA" },
                new() { Name = "Paraíba", Uf = "PB" },
                new() { Name = "Paraná", Uf = "PR" },
                new() { Name = "Pernambuco", Uf = "PE" },
                new() { Name = "Piauí", Uf = "PI" },
                new() { Name = "Rio de Janeiro", Uf = "RJ" },
                new() { Name = "Rio Grande do Norte", Uf = "RN" },
                new() { Name = "Rio Grande do Sul", Uf = "RS" },
                new() { Name = "Rondônia", Uf = "RO" },
                new() { Name = "Roraima", Uf = "RR" },
                new() { Name = "Santa Catarina", Uf = "SC" },
                new() { Name = "São Paulo", Uf = "SP" },
                new() { Name = "Sergipe", Uf = "SE" },
                new() { Name = "Tocantins", Uf = "TO" }
            };

            foreach (var state in states)
            {
                context.AuthStates.Add(state);
            }
        }
    }

    private static void SeedModules(DefaultContext context)
    {
        if (!context.AuthModules.Any())
        {
            var modules = new List<ModuleModel>();
            var sortOrder = 1;

            foreach (var moduleType in Enum.GetValues<ModuleType>())
            {
                modules.Add(new ModuleModel
                {
                    Name = moduleType.ToString(),
                    Type = moduleType,
                    Price = 30m,
                    IsActive = true,
                    SortOrder = sortOrder++,
                    Description = $"Module {moduleType}"
                });
            }

            foreach (var module in modules)
            {
                context.AuthModules.Add(module);
            }
        }
    }
}
