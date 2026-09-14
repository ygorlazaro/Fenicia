using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;

namespace Fenicia.Auth.Domains.Company.Interfaces;

public interface ICompanyService
{
    Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> GetCompaniesByUserAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Guid companyId, Guid userId, string name, CancellationToken cancellationToken = default);

    Task<CompanyModel?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

    Task<CompanyModel> InsertAsync(CompanyModel company, CancellationToken cancellationToken = default);
}