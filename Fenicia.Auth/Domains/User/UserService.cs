using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.User;

public class UserService(
    IMapper mapper,
    ILogger<UserService> logger,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    ICompanyRepository companyRepository,
    ISecurityService securityService
) : IUserService
{
    public async Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request)
    {
        logger.LogInformation("Getting user for login");
        var user = await userRepository.GetByEmailAndCnpjAsync(request.Email, request.Cnpj);

        if (user is null)
        {
            logger.LogInformation("Invalid login - {email}", request.Email);

            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.InvalidUsernameOrPassword
            );
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword.Data)
        {
            var response = mapper.Map<UserResponse>(user);

            return new ApiResponse<UserResponse>(response);
        }

        logger.LogInformation("Invalid login - {email}", request.Email);

        return new ApiResponse<UserResponse>(
            null,
            HttpStatusCode.BadRequest,
            TextConstants.InvalidUsernameOrPassword
        );
    }

    public async Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request)
    {
        logger.LogInformation("Creating new user");
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(
            request.Company.Cnpj
        );

        if (isExistingUser)
        {
            logger.LogInformation("User already exists - {email}", request.Email);

            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.EmailExists
            );
        }

        if (isExistingCompany)
        {
            logger.LogInformation("Company already exists - {cnpj}", request.Company.Cnpj);

            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.BadRequest,
                TextConstants.CompanyExists
            );
        }

        var hashedPassword = securityService.HashPassword(request.Password).Data;

        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword!,
            Name = request.Name
        };

        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(
            new CompanyModel { Name = request.Company.Name, Cnpj = request.Company.Cnpj }
        );
        var adminRole = await roleRepository.GetAdminRoleAsync();

        if (adminRole is null)
        {
            logger.LogCritical("Missing admin role. Please check database.");

            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.InternalServerError,
                TextConstants.MissingAdminRole
            );
        }

        user.UsersRoles =
        [
            new UserRoleModel
            {
                User = user,
                Company = company,
                Role = adminRole
            }
        ];

        await userRepository.SaveAsync();

        var response = mapper.Map<UserResponse>(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId)
    {
        logger.LogInformation("Checking if user exists in company");
        var response = await userRoleRepository.ExistsInCompanyAsync(userId, companyId);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId)
    {
        logger.LogInformation("Getting user for refresh");
        var user = await userRepository.GetUserForRefreshTokenAsync(userId);
        var response = mapper.Map<UserResponse>(user);

        if (user is null)
        {
            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.Unauthorized,
                TextConstants.PermissionDenied
            );
        }

        return new ApiResponse<UserResponse>(response);
    }
}
