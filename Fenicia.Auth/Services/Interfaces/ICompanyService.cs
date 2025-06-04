using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface ICompanyService
{
    Task<ServiceResponse<CompanyResponse>> GetByCnpjAsync(string cnpj);
    Task<ServiceResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, int page, int perPage);
    Task<ServiceResponse<CompanyResponse>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company);
    Task<ServiceResponse<int>> CountByUserIdAsync(Guid userId);
}