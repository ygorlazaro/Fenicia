using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(CompanyRepository repository, UserRoleService userRoleService)
{
    public async Task<Pagination<IEnumerable<GetCompaniesByUserResponse>>> GetCompaniesByUserAsync(Guid userId, int page, int perPage, CancellationToken ct)
    {
        if (perPage <= 0)
        {
            throw new InvalidRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var userRoles = await userRoleService.GetUserRolesAsync(userId, page, perPage, ct);
        var total = await userRoleService.CountUserRolesAsync(userId, ct);

        var result = userRoles.Select(ur => ur.MapToGetCompaniesByUserResponse());

        return new Pagination<IEnumerable<GetCompaniesByUserResponse>>(result, total, page, perPage);
    }

    public async Task UpdateAsync(Guid companyId, Guid userId, string name, CancellationToken ct)
    {
        var company = await repository.AnyActiveAsync(companyId, ct) ?? throw new ItemNotExistsException(ExceptionMessages.CompanyNotFoundMessage);
        var isAdmin = await userRoleService.IsAdminAsync(userId, companyId, ct);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        company.Name = name;
        await repository.UpdateAsync(company.Id, company, ct);
    }

    public async Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken ct)
    {
        return await repository.CheckExistsAsync(cnpj, onlyActive, ct);
    }

    public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct)
    {
        return await repository.AnyActiveAsync(companyId, ct);
    }

    public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct)
    {
        return await repository.AnyAsync(companyId, ct);
    }

    public async Task<CompanyModel?> GetByIdAsync(Guid companyId, CancellationToken ct)
    {
        return await repository.GetByIdAsync(companyId, ct);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken ct)
    {
        return await repository.GetByCnpjAsync(cnpj, ct);
    }

    public async Task<CompanyModel> InsertAsync(CompanyModel company, CancellationToken ct)
    {
        return await repository.InsertAsync(company, ct);
    }
}
