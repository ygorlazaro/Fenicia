using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.State;

public class StateService(IStateRepository stateRepository) : IStateService
{
    public async Task<List<StateResponse>> LoadStatesAtDatabaseAsync(CancellationToken cancellationToken)
    {
        var states = new List<StateModel>
                     {
                         new () { Name = "Acre", Uf = "AC" },
                         new () { Name = "Alagoas", Uf = "AL" },
                         new () { Name = "Amapá", Uf = "AP" },
                         new () { Name = "Amazonas", Uf = "AM" },
                         new () { Name = "Bahia", Uf = "BA" },
                         new () { Name = "Ceará", Uf = "CE" },
                         new () { Name = "Distrito Federal", Uf = "DF" },
                         new () { Name = "Espírito Santo", Uf = "ES" },
                         new () { Name = "Goiás", Uf = "GO" },
                         new () { Name = "Maranhão", Uf = "MA" },
                         new () { Name = "Mato Grosso", Uf = "MT" },
                         new () { Name = "Mato Grosso do Sul", Uf = "MS" },
                         new () { Name = "Minas Gerais", Uf = "MG" },
                         new () { Name = "Pará", Uf = "PA" },
                         new () { Name = "Paraíba", Uf = "PB" },
                         new () { Name = "Paraná", Uf = "PR" },
                         new () { Name = "Pernambuco", Uf = "PE" },
                         new () { Name = "Piauí", Uf = "PI" },
                         new () { Name = "Rio de Janeiro", Uf = "RJ" },
                         new () { Name = "Rio Grande do Norte", Uf = "RN" },
                         new () { Name = "Rio Grande do Sul", Uf = "RS" },
                         new () { Name = "Rondônia", Uf = "RO" },
                         new () { Name = "Roraima", Uf = "RR" },
                         new () { Name = "Santa Catarina", Uf = "SC" },
                         new () { Name = "São Paulo", Uf = "SP" },
                         new () { Name = "Sergipe", Uf = "SE" },
                         new () { Name = "Tocantins", Uf = "TO" }
                     };

        var response = await stateRepository.LoadStatesAtDatabaseAsync(states, cancellationToken);
        var mapped = StateResponse.Convert(response);

        return mapped;
    }
}
