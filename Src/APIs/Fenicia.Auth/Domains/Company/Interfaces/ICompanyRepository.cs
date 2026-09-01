using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Company.Interfaces;

public interface ICompanyRepository : IRepository<CompanyModel>
{
    Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

    Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken cancellationToken = default);
}
