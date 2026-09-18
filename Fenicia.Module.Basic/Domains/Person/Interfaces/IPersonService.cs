using Fenicia.Common.DTOs.Basic.Person;

namespace Fenicia.Module.Basic.Domains.Person.Interfaces;

public interface IPersonService
{
    Task<GetPersonByIdResponse> InsertAsync(UpsertPersonCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<GetPersonByIdResponse?> UpdateAsync(
        Guid id,
        UpsertPersonCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
