using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;

namespace Fenicia.Auth.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponse?> GetByCnpjAsync(string cnpj);
    Task<List<CompanyResponse>> GetByUserIdAsync(Guid userId, int page, int perPage);
    Task<CompanyResponse?> PatchAsync(Guid companyId, Guid userId, CompanyRequest company);
    Task<int> CountByUserIdAsync(Guid userId);
}