using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService,
    IRoleService roleService) : ICompanyService
{
    public async Task<CompanyResponse> GetByCnpjAsync(string cnpj, CancellationToken ct)
    {
        var company = await companyRepository.GetByCnpjAsync(cnpj, false, ct)
                      ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);

        return CompanyMapper.Map(company);
    }

    public async Task<List<CompanyResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var companies = await companyRepository.GetByUserIdAsync(userId, true, ct, page, perPage);
        var companiesResponse = CompanyMapper.Map(companies);

        foreach (var company in companiesResponse)
        {
            var role = await roleService.GetByUserAndCompanyAsync(userId, company.Id, ct);

            if (role is null) continue;

            company.Role = role;
        }

        return companiesResponse;
    }

    public async Task<CompanyResponse?> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company,
        CancellationToken ct)
    {
        var existing = await companyRepository.CheckCompanyExistsAsync(companyId, true, ct);

        if (!existing) throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);

        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin", ct);

        if (!hasAdminRole) throw new PermissionDeniedException(TextConstants.PermissionDeniedMessage);

        var companyToUpdate = CompanyMapper.Map(company);

        companyToUpdate.Id = companyId;

        companyRepository.Update(companyToUpdate);
        var saved = await companyRepository.SaveChangesAsync(ct);

        return saved == 0
            ? throw new NotSavedException(TextConstants.ThereWasAnErrorEditingMessage)
            : CompanyMapper.Map(companyToUpdate);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await companyRepository.CountByUserIdAsync(userId, true, ct);
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken ct)
    {
        return await companyRepository.GetCompaniesAsync(userId, true, ct);
    }
}