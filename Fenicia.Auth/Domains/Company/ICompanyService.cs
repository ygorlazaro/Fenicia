using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Company;

public interface ICompanyService
{
    Task<CompanyResponse> GetByCnpjAsync(string cnpj, CancellationToken ct);

    Task<List<CompanyResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct, int page = 1, int perPage = 10);

    Task<CompanyResponse?> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken ct);

    Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct);

    Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken ct);
}