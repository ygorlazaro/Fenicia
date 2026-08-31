using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(CompanyRepository repository, UserRoleService userRoleService)
{
    public async Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> GetCompaniesByUserAsync(Guid userId, int page, int perPage, CancellationToken cancellationToken = default)
    {
        if (perPage <= 0)
        {
            throw new InvalidRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var userRoles = await userRoleService.GetUserRolesAsync(userId, page, perPage, cancellationToken);
        var total = await userRoleService.CountUserRolesAsync(userId, cancellationToken);

        var result = userRoles.Select(ur => ur.MapToGetCompaniesByUserResponse());

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(result, total, page, perPage);
    }

    public async Task UpdateAsync(Guid companyId, Guid userId, string name, CancellationToken cancellationToken = default)
    {
        var company = await repository.AnyActiveAsync(companyId, cancellationToken) ?? throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);
        var isAdmin = await userRoleService.IsAdminAsync(userId, companyId, cancellationToken);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        company.Name = name;
        await repository.UpdateAsync(company.Id, company, cancellationToken);
    }

    public async Task<CompanyModel?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(companyId, cancellationToken);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await repository.GetByCnpjAsync(cnpj, cancellationToken);
    }

    public async Task<CompanyModel> InsertAsync(CompanyModel company, CancellationToken cancellationToken = default)
    {
        return await repository.InsertAsync(company, cancellationToken);
    }
}
