using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Company;

public interface ICompanyRepository : IBaseRepository<CompanyModel>
{
    Task<bool> CheckCompanyExistsAsync(Guid companyId, bool onlyActive, CancellationToken ct);

    Task<bool> CheckCompanyExistsAsync(string cnpj, bool onlyActive, CancellationToken ct);

    Task<CompanyModel?> GetByCnpjAsync(string cnpj, bool onlyActive, CancellationToken ct);

    Task<List<CompanyModel>> GetByUserIdAsync(
        Guid userId,
        bool onlyActive,
        CancellationToken ct,
        int page = 1,
        int perPage = 10);

    Task<int> CountByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct);

    Task<List<Guid>> GetCompaniesAsync(Guid userId, bool onlyActive, CancellationToken ct);
}