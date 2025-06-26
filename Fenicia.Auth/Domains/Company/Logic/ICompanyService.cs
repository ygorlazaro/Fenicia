using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company.Logic;

public interface ICompanyService
{
    Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);
    Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken,
        int page,
        int perPage
    );
    Task<ApiResponse<CompanyResponse>> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company,
        CancellationToken cancellationToken
    );
    Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
