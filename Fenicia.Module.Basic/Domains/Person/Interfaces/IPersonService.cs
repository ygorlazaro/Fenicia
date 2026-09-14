using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Person.Interfaces;

public interface IPersonService
{
    Task<PersonModel> InsertAsync(PersonModel person, Guid companyId, CancellationToken cancellationToken = default);

    Task<PersonModel?> UpdateAsync(
        Guid id,
        PersonModel person,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<PersonModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}