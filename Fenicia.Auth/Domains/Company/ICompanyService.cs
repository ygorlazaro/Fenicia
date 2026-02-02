using Fenicia.Common;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Company;

public interface ICompanyService
{
    Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);

    Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);

    Task<ApiResponse<CompanyResponse?>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken);

    Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken);
}
