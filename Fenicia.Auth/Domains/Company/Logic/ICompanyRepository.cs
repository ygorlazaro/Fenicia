namespace Fenicia.Auth.Domains.Company.Logic;

using System.Diagnostics.CodeAnalysis;

using Data;

public interface ICompanyRepository
{
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    CompanyModel Add(CompanyModel company);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<int> SaveAsync(CancellationToken cancellationToken);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    CompanyModel PatchAsync(CompanyModel company);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMemberInSuper.Global")]
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
