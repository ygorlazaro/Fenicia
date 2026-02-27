using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

namespace Fenicia.Auth.Domains.User.CreateNewUser;

public class CreateNewUserHandler(
    DefaultContext context,
    CheckUserExistsHandler checkUserExistsHandler,
    CheckCompanyExistsHandler checkCompanyExistsHandler,
    HashPasswordHandler hashPasswordHandler,
    GetAdminRoleHandler getAdminRoleHandler)
{
    public async Task<CreateNewUserResponse> Handle(CreateNewUserQuery request, CancellationToken ct)
    {
        var isExistingUser = await checkUserExistsHandler.Handle(request.Email, ct);
        var checkUserExistsQuery = new CheckCompanyExistsQuery(request.Company.Cnpj, true);
        var isExistingCompany = await checkCompanyExistsHandler.Handle(checkUserExistsQuery, ct);

        if (isExistingUser)
        {
            throw new ArgumentException(TextConstants.EmailExistsMessage);
        }

        if (isExistingCompany)
        {
            throw new ArgumentException(TextConstants.CompanyExistsMessage);
        }

        var hashedPassword = hashPasswordHandler.Handle(request.Password);
        var userRequest = new AuthUser
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };
        context.AuthUsers.Add(userRequest);

        var companyRequest = new AuthCompany
        {
            Name = request.Company.Name,
            Cnpj = request.Company.Cnpj,
            TimeZone = request.Company.TimeZone
        };

        context.Companies.Add(companyRequest);

        var adminRole = await getAdminRoleHandler.Handle(ct)
                        ?? throw new ArgumentException(TextConstants.MissingAdminRoleMessage);
        var userRole = new AuthUserRole
        {
            UserId = userRequest.Id,
            Company = companyRequest,
            RoleId = adminRole.Id
        };

        context.UserRoles.Add(userRole);

        await context.SaveChangesAsync(ct);

        var companyResponse =
            new CreateNewUserCompanyResponse(companyRequest.Id, companyRequest.Name, companyRequest.Cnpj);

        return new CreateNewUserResponse(userRequest.Id, userRequest.Name, userRequest.Email, companyResponse);
    }
}
