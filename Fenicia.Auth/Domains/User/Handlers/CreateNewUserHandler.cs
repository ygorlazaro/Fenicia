using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.User.Handlers;

public class CreateNewUserHandler(
    DefaultContext db)
{
    public async Task<CreateNewUserResponse> Handle(CreateNewUserCommand command, CancellationToken ct)
    {
        await ValidateAsync(command, ct);

        var (user, company) = await PersistAsync(command, ct);

        var companyResponse =
            new CreateNewUserCompanyResponse(company.Id, company.Name, company.Cnpj);

        return new CreateNewUserResponse(user.Id, user.Name, user.Email, companyResponse);
    }

    private async Task<(UserModel userRequest, CompanyModel companyRequest)> PersistAsync(CreateNewUserCommand command, CancellationToken ct)
    {
        var existingUser = await db.AuthUsers.AnyEmailAsync(command.Email, ct);

        if (existingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }
        
        var existingCompany = await db.AuthCompanies.AnyCnpjAsync(command.Company.Cnpj, ct);

        if (existingCompany)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyExists);
        }
        
        var hashedPassword = command.Password.Hash();
        var userRequest = new UserModel
        {
            Email = command.Email,
            Password = hashedPassword,
            Name = command.Name
        };

        db.AuthUsers.Add(userRequest);

        var companyRequest = new CompanyModel
        {
            Name = command.Company.Name,
            Cnpj = command.Company.Cnpj
        };

        db.AuthCompanies.Add(companyRequest);

        var adminRole = await db.AuthRoles.GetRoleAsync("Admin", ct)
                        ?? throw new InvalidRequestException(ExceptionMessages.AdminRoleNotFound);
        var userRole = new UserRoleModel
        {
            UserId = userRequest.Id,
            Company = companyRequest,
            RoleId = adminRole.Id
        };

        db.AuthUserRoles.Add(userRole);

        await db.SaveChangesAsync(ct);
        return (userRequest, companyRequest);
    }

    private async Task ValidateAsync(CreateNewUserCommand request, CancellationToken ct)
    {
        var isExistingUser = await db.AuthUsers.AnyEmailAsync(request.Email, ct);
        var isExistingCompany = await db.AuthCompanies.AnyCnpjAsync(request.Company.Cnpj, ct); 
            
        if (isExistingUser)
        {
            throw new InvalidRequestException(ExceptionMessages.EmailAlreadyExists);
        }

        if (isExistingCompany)
        {
            throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundWithCNPJ);
        }
    }
}
