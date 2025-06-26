using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Company.Logic;
using Fenicia.Auth.Domains.LoginAttempt.Logic;
using Fenicia.Auth.Domains.Role.Logic;
using Fenicia.Auth.Domains.Security.Logic;
using Fenicia.Auth.Domains.Token.Logic;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Auth.Domains.UserRole.Logic;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.User.Logic;

public class UserService(
    IMapper mapper,
    ILogger<UserService> logger,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    ICompanyRepository companyRepository,
    ISecurityService securityService,
    ILoginAttemptService loginAttemptService
) : IUserService
{
    public async Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user for login");

        var attempts = await loginAttemptService.GetAttemptsAsync(request.Email, cancellationToken);

        if (attempts >= 5)
        {
            logger.LogWarning("User blocked temporarily - {email}", request.Email);
            return new ApiResponse<UserResponse>(
                null,
                HttpStatusCode.TooManyRequests,
                "Muitas tentativas. Tente novamente mais tarde."
            );
        }

        var user = await userRepository.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Invalid login - {email}", request.Email);
            await loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);
            return new ApiResponse<UserResponse>(null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword.Data)
        {
            await loginAttemptService.ResetAttemptsAsync(request.Email, cancellationToken);
            var response = mapper.Map<UserResponse>(user);
            return new ApiResponse<UserResponse>(response);
        }

        logger.LogWarning("Invalid password - {email}", request.Email);
        await loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);

        return new ApiResponse<UserResponse>(
            null,
            HttpStatusCode.BadRequest,
            TextConstants.InvalidUsernameOrPassword
        );
    }

    public async Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new user");
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email, cancellationToken);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(
            request.Company.Cnpj, cancellationToken
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
        var adminRole = await roleRepository.GetAdminRoleAsync(cancellationToken);

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

        await userRepository.SaveAsync(cancellationToken);

        var response = mapper.Map<UserResponse>(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if user exists in company");
        var response = await userRoleRepository.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user for refresh");
        var user = await userRepository.GetUserForRefreshTokenAsync(userId, cancellationToken);
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

    public async Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Finding id of user {email}", email);

        var userId = await userRepository.GetUserIdFromEmailAsync(email, cancellationToken);

        if (userId is null)
        {
            return new ApiResponse<UserResponse>(null, HttpStatusCode.NotFound,
                TextConstants.ItemNotFound);
        }

        var response = new UserResponse
        {
            Id = userId.Value
        };

        return new ApiResponse<UserResponse>(response);

    }

    public async Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Changing password for user {userId}", userId);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);


        if (user is null)
        {
            logger.LogInformation("User not found {userId}", userId);

            return new ApiResponse<UserResponse>(null, HttpStatusCode.NotFound,
                TextConstants.ItemNotFound);
        }

        var hashedPassword = securityService.HashPassword(password).Data;
        user.Password = hashedPassword!;
        await userRepository.SaveAsync(cancellationToken);

        return new ApiResponse<UserResponse>(mapper.Map<UserResponse>(user));
    }
}
