using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company;

public interface ICompanyService
{
    Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj);
    Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        int page,
        int perPage
    );
    Task<ApiResponse<CompanyResponse>> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company
    );
    Task<ApiResponse<int>> CountByUserIdAsync(Guid userId);
}
