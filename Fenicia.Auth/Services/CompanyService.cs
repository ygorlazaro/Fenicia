using System.Net;
using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.Extensions.FileProviders;

namespace Fenicia.Auth.Services;

public class CompanyService(
    IMapper mapper,
    ILogger<CompanyService> logger,
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService) : ICompanyService
{
    public async Task<ServiceResponse<CompanyResponse>> GetByCnpjAsync(string cnpj)
    {
        logger.LogInformation("Getting company {cnpj}", [cnpj]);
        var company = await companyRepository.GetByCnpjAsync(cnpj);

        if (company is null)
        {
            return new ServiceResponse<CompanyResponse>(null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var response = mapper.Map<CompanyResponse>(company);

        return new ServiceResponse<CompanyResponse>(response);
    }

    public async Task<ServiceResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, int page = 1, int perPage = 10)
    {
        logger.LogInformation("Getting companies by user {userId}", [userId]);
        var companies = await companyRepository.GetByUserIdAsync(userId, page, perPage);
        var response = mapper.Map<List<CompanyResponse>>(companies);

        return new ServiceResponse<List<CompanyResponse>>(response);
    }

    public async Task<ServiceResponse<CompanyResponse>> PatchAsync(Guid companyId, Guid userId, CompanyRequest company)
    {
        logger.LogInformation("Patching company {companyId}", [companyId]);
        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin");

        if (!hasAdminRole.Data)
        {
            logger.LogWarning("User {userId} does not have admin role", [userId]);

            return new ServiceResponse<CompanyResponse>(null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
        }

        var companyToUpdate = mapper.Map<CompanyRequest, CompanyModel>(company);

        companyToUpdate.Id = companyId;

        var updatedCompany = await companyRepository.PatchAsync(companyToUpdate);

        if (updatedCompany is null)
        {
            return new ServiceResponse<CompanyResponse>(null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var response = mapper.Map<CompanyResponse>(updatedCompany);

        return new ServiceResponse<CompanyResponse>(response);
    }

    public async Task<ServiceResponse<int>> CountByUserIdAsync(Guid userId)
    {
        var response = await companyRepository.CountByUserIdAsync(userId);

        return new ServiceResponse<int>(response);
    }
}