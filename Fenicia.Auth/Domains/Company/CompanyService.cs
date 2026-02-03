using System.Net;

using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(ICompanyRepository companyRepository, IUserRoleService userRoleService) : ICompanyService
{
    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByCnpjAsync(cnpj, cancellationToken);

        if (company is null)
        {
            return new ApiResponse<CompanyResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var mapped = new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            Language = company.Language,
            TimeZone = company.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        };

        return new ApiResponse<CompanyResponse>(mapped);
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var companies = await companyRepository.GetByUserIdAsync(userId, cancellationToken, page, perPage);
        var mapped = companies.Select(company => new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            Language = company.Language,
            TimeZone = company.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        }).ToList();

        return new ApiResponse<List<CompanyResponse>>(mapped);
    }

    public async Task<ApiResponse<CompanyResponse?>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken)
    {
        var existing = await companyRepository.CheckCompanyExistsAsync(companyId, cancellationToken);

        if (!existing)
        {
            return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin", cancellationToken);

        if (!hasAdminRole.Data)
        {
            return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDeniedMessage);
        }

        var companyToUpdate = CompanyModel.Convert(company);

        companyToUpdate.Id = companyId;

        var updatedCompany = companyRepository.PatchAsync(companyToUpdate);
        var saved = await companyRepository.SaveAsync(cancellationToken);

        if (saved == 0)
        {
            return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFoundMessage);
        }

        var response = new CompanyResponse
        {
            Id = updatedCompany.Id,
            Name = updatedCompany.Name,
            Cnpj = updatedCompany.Cnpj,
            Language = updatedCompany.Language,
            TimeZone = updatedCompany.TimeZone,
            Role = new RoleModel { Name = string.Empty }
        };

        return new ApiResponse<CompanyResponse?>(response);
    }

    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var response = await companyRepository.CountByUserIdAsync(userId, cancellationToken);

        return new ApiResponse<int>(response);
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await companyRepository.GetCompaniesAsync(userId, cancellationToken);
    }
}
