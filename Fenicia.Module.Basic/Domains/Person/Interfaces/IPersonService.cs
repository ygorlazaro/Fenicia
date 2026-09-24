using Fenicia.Common.DTOs.Auth.Person;

namespace Fenicia.Module.Basic.Domains.Person.Interfaces;

public interface IPersonService
{
    Task<PersonResponse> InsertAsync(PersonRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task<PersonResponse?> UpdateAsync(
        Guid id,
        PersonRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
