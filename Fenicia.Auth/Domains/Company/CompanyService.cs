using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(ICompanyRepository companyRepository, IUserRoleService userRoleService) : ICompanyService
{
    public async Task<CompanyResponse> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByCnpjAsync(cnpj, onlyActive: false, cancellationToken) ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);

        return CompanyModel.Convert(company);
    }

    public async Task<List<CompanyResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var companies = await companyRepository.GetByUserIdAsync(userId, onlyActive: true, cancellationToken, page, perPage);

        return [.. companies.Select(company => new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            Language = company.Language,
            TimeZone = company.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        })];
    }

    public async Task<CompanyResponse?> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken)
    {
        var existing = await companyRepository.CheckCompanyExistsAsync(companyId, onlyActive: true, cancellationToken);

        if (!existing)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin", cancellationToken);

        if (!hasAdminRole)
        {
            throw new PermissionDeniedException(TextConstants.PermissionDeniedMessage);
        }

        var companyToUpdate = CompanyModel.Convert(company);

        companyToUpdate.Id = companyId;

        companyRepository.Update(companyToUpdate);
        var saved = await companyRepository.SaveChangesAsync(cancellationToken);

        if (saved == 0)
        {
            throw new NotSavedException(TextConstants.ThereWasAnErrorEditingMessage);
        }

        return new CompanyResponse
        {
            Id = companyToUpdate.Id,
            Name = companyToUpdate.Name,
            Cnpj = companyToUpdate.Cnpj,
            Language = companyToUpdate.Language,
            TimeZone = companyToUpdate.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        };
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await companyRepository.CountByUserIdAsync(userId, onlyActive: true, cancellationToken);
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await companyRepository.GetCompaniesAsync(userId, onlyActive: true, cancellationToken);
    }
}
