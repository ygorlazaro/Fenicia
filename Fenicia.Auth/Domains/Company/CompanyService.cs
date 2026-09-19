using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Company;

public sealed class CompanyService(
    CompanyMapper mapper,
    ICompanyRepository repository,
    IUserRoleService userRoleService) : ICompanyService
{
    public async Task<Pagination<IEnumerable<CompanyByUserResponse>>> GetCompaniesByUserAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        if (perPage <= 0)
        {
            throw new InvalidRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var userRoles = await userRoleService.GetUserRolesAsync(userId, page, perPage, cancellationToken);
        var total = await userRoleService.CountUserRolesAsync(userId, cancellationToken);

        var activeUserRoles = userRoles.Where(ur => ur.Company.IsActive).ToList();
        var result = activeUserRoles.Select(mapper.MapToCompaniesByUserResponse);

        return new Pagination<IEnumerable<CompanyByUserResponse>>(result, total, page, perPage);
    }

    public async Task UpdateAsync(
        Guid companyId,
        Guid userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var isAdmin = await userRoleService.IsAdminAsync(userId, companyId, cancellationToken);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        var company = await repository.AnyActiveAsync(companyId, cancellationToken) ??
                      throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);
        company.Name = name;
        await repository.UpdateAsync(company.Id, company, cancellationToken);
    }

    public async Task<CompanyResponse?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetByIdAsync(companyId, cancellationToken);

        return company is null ? null : mapper.MapToCompanyResponse(company);
    }

    public async Task<CompanyResponse?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetByCnpjAsync(cnpj, cancellationToken);

        return company is null ? null : mapper.MapToCompanyResponse(company);
    }

    public async Task<CompanyResponse> InsertAsync(CompanyModel company, CancellationToken cancellationToken = default)
    {
        var result = await repository.InsertAsync(company, cancellationToken);

        return mapper.MapToCompanyResponse(result);
    }
}