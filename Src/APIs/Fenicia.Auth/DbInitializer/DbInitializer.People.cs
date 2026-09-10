using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedAddresses(DefaultContext context)
    {
        if (context.AuthAddresses.Any())
        {
            return;
        }

        var cities = new[]
        {
            "São Paulo", "Rio de Janeiro", "Belo Horizonte", "Curitiba", "Porto Alegre",
            "Salvador", "Recife", "Fortaleza", "Brasília", "Manaus",
            "Belém", "Goiânia", "Campinas", "São Luís", "Maceió",
            "Natal", "Teresina", "Cuiabá", "Campo Grande", "Florianópolis",
            "João Pessoa", "Aracaju", "Palmas", "Macapá", "Boa Vista",
            "Porto Velho", "Rio Branco", "Vitória", "São Bernardo do Campo", "Santo André",
            "Osasco", "Sorocaba", "Ribeirão Preto", "Uberlândia", "Contagem",
            "Juiz de Fora", "Joinville", "Londrina", "Caxias do Sul", "Maringá"
        };

        var states = context.AuthStates.ToList();
        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            int cityIndex;
            if (i <= 40)
            {
                cityIndex = i - 1;
            }
            else
            {
                cityIndex = i % 40;
            }

            var state = states[random.Next(states.Count)];

            var address = new AddressModel
            {
                Street = $"Rua {(i % 50) + 1}",
                Number = ((i % 1000) + 1).ToString(),
                Complement = i % 3 == 0 ? $"Apto {(i % 20) + 1}" : null,
                Neighborhood = $"Bairro {(i % 20) + 1}",
                ZipCode = ((i % 90000000) + 10000000).ToString("D8"),
                StateId = state.Id,
                City = cities[cityIndex],
                Country = "Brasil",
                AddressType = (AddressType)((i % 3) + 1),
                Latitude = -23.55 + ((random.NextDouble() - 0.5) * 0.5),
                Longitude = -46.63 + ((random.NextDouble() - 0.5) * 0.5),
                IsDefault = i % 10 == 0,
                Observation = i % 5 == 0 ? $"Observação endereço {i}" : null,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthAddresses.Add(address);
        }
    }

    private static void SeedCompanyAddress(DefaultContext context)
    {
        var company = context.AuthCompanies.FirstOrDefault(x => x.AddressId == null);
        if (company == null)
        {
            return;
        }

        var address = context.AuthAddresses.FirstOrDefault();
        if (address != null)
        {
            company.AddressId = address.Id;
        }
    }

    private static void SeedPeople(DefaultContext context)
    {
        if (context.BasicPeople.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();

        for (var i = 1; i <= 200; i++)
        {
            var name = (i % 10) switch
            {
                0 => $"Pessoa {i} Silva",
                1 => $"Pessoa {i} Santos",
                2 => $"Pessoa {i} Oliveira",
                3 => $"Pessoa {i} Souza",
                4 => $"Pessoa {i} Lima",
                5 => $"Pessoa {i} Pereira",
                6 => $"Pessoa {i} Costa",
                7 => $"Pessoa {i} Rodrigues",
                8 => $"Pessoa {i} Almeida",
                _ => $"Pessoa {i} Ferreira"
            };

            var document = i % 3 == 0 ? ((i % 90000000000) + 10000000000).ToString("D11") : null;
            var email = $"pessoa{i}@email.com";
            var phone = $"({(i % 90) + 10}) 9{(i % 90000000) + 10000000:D8}";
            var dateOfBirth = new DateTime(1980, 1, 1).AddDays(i % 12000);
            var photoUrl = i % 4 == 0 ? $"https://i.pravatar.cc/150?u=person-{i}" : null;
            var notes = i % 7 == 0 ? $"Observações da pessoa {i}" : null;

            var person = new PersonModel
            {
                CompanyId = companyId,
                Name = name,
                Document = document,
                Email = email,
                PhoneNumber = phone,
                DateOfBirth = dateOfBirth,
                PhotoUrl = photoUrl,
                Notes = notes
            };

            context.BasicPeople.Add(person);
        }
    }

    private static void SeedPersonAddresses(DefaultContext context)
    {
        if (context.BasicPersonAddresses.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var people = context.BasicPeople.ToList();
        var addresses = context.AuthAddresses.ToList();

        if (!people.Any() || !addresses.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

#pragma warning disable CA5394
        for (var i = 0; i < 100; i++)
        {
            var personAddress = new PersonAddressModel
            {
                CompanyId = companyId,
                PersonId = people[i % 200].Id,
                AddressId = addresses[i % 100].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.BasicPersonAddresses.Add(personAddress);
        }
#pragma warning restore CA5394
    }
}
