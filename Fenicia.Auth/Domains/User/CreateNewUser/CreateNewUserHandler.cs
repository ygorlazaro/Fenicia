using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Migrations.Services;

namespace Fenicia.Auth.Domains.User.CreateNewUser;

public class CreateNewUserHandler(
    AuthContext context,
    CheckUserExistsHandler checkUserExistsHandler,
    CheckCompanyExistsHandler checkCompanyExistsHandler,
    HashPasswordHandler hashPasswordHandler,
    GetAdminRoleHandler getAdminRoleHandler,
    IMigrationService migrationService)
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
        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword,
            Name = request.Name
        };
        context.Users.Add(userRequest);

        var companyRequest = new CompanyModel
        {
            Name = request.Company.Name,
            Cnpj = request.Company.Cnpj,
            TimeZone = request.Company.TimeZone
        };

        context.Companies.Add(companyRequest);

        var adminRole = await getAdminRoleHandler.Handle(ct)
                        ?? throw new ArgumentException(TextConstants.MissingAdminRoleMessage);
        var userRole = new UserRoleModel
        {
            UserId = userRequest.Id,
            Company = companyRequest,
            RoleId = adminRole.Id
        };

        context.UserRoles.Add(userRole);

        await context.SaveChangesAsync(ct);

        await migrationService.RunMigrationsAsync(companyRequest.Id, [ModuleType.Basic], ct);

        var companyResponse =
            new CreateNewUserCompanyResponse(companyRequest.Id, companyRequest.Name, companyRequest.Cnpj);

        return new CreateNewUserResponse(userRequest.Id, userRequest.Name, userRequest.Email, companyResponse);
    }
}