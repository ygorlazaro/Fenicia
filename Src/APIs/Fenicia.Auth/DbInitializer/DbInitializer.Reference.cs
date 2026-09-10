using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedCompanies(DefaultContext context)
    {
        if (context.AuthCompanies.Any())
        {
            return;
        }

        context.AuthCompanies.Add(new CompanyModel
        {
            Id = Guid.Parse("d6f7e2c6-2986-47a5-8884-c3c249546ba7"),
            Name = "Gato Ninja",
            Cnpj = "23351185000184"
        });

        context.SaveChanges();
    }

    private static void SeedStates(DefaultContext context)
    {
        if (context.AuthStates.Any())
        {
            return;
        }

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

    private static void SeedModules(DefaultContext context)
    {
        if (context.AuthModules.Any())
        {
            return;
        }

        var sortOrder = 1;

        var modules = Enum.GetValues<ModuleType>()
            .Select(moduleType => new ModuleModel
            {
                Name = moduleType.ToString(),
                Type = moduleType,
                Price = 30m,
                IsActive = true,
                SortOrder = sortOrder++,
                Description = $"Module {moduleType}"
            })
            .ToList();

        foreach (var module in modules)
        {
            context.AuthModules.Add(module);
        }
    }
}
