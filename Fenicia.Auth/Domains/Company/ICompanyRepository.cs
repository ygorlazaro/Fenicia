namespace Fenicia.Auth.Domains.Company;

using Fenicia.Common.Database.Models.Auth;

public interface ICompanyRepository
{
    Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken);

    Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken);

    CompanyModel Add(CompanyModel company);

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);

    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);

    CompanyModel PatchAsync(CompanyModel company);

    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
