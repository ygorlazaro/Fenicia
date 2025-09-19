namespace Fenicia.Auth.Domains.User;

using System.Net;

using Common;
using Common.Database.Requests;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Responses;
using Company;
using LoginAttempt;
using Role;
using Security;
using UserRole;

public class UserService : IUserService
{
    private readonly ILogger<UserService> logger;
    private readonly IUserRepository userRepository;
    private readonly IRoleRepository roleRepository;
    private readonly IUserRoleRepository userRoleRepository;
    private readonly ICompanyRepository companyRepository;
    private readonly ISecurityService securityService;
    private readonly ILoginAttemptService loginAttemptService;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService, ILoginAttemptService loginAttemptService)
    {
        this.logger = logger;
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
        this.userRoleRepository = userRoleRepository;
        this.companyRepository = companyRepository;
        this.securityService = securityService;
        this.loginAttemptService = loginAttemptService;
    }

    public async Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting login process for user {Email}", request.Email);

        var attempts = await loginAttemptService.GetAttemptsAsync(request.Email, cancellationToken);

        if (attempts >= 5)
        {
            logger.LogWarning("User blocked temporarily - {email}", request.Email);
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.TooManyRequests, "Muitas tentativas. Tente novamente mais tarde.");
        }

        var user = await userRepository.GetByEmailAndCnpjAsync(request.Email, request.Cnpj, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Invalid login - {email}", request.Email);
            await loginAttemptService.IncrementAttemptsAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (isValidPassword.Data)
        {
            await loginAttemptService.ResetAttemptsAsync(request.Email, cancellationToken);
            var response = UserResponse.Convert(user);
            return new ApiResponse<UserResponse>(response);
        }

        logger.LogWarning("Invalid password - {email}", request.Email);
        await loginAttemptService.IncrementAttemptsAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, val2: 5)), cancellationToken);

        return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.InvalidUsernameOrPassword);
    }

    public async Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting user creation process for {Email}", request.Email);
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email, cancellationToken);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj, cancellationToken);

        if (isExistingUser)
        {
            logger.LogInformation("User already exists - {email}", request.Email);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.EmailExists);
        }

        if (isExistingCompany)
        {
            logger.LogInformation("Company already exists - {cnpj}", request.Company.Cnpj);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.BadRequest, TextConstants.CompanyExists);
        }

        var hashedPassword = securityService.HashPassword(request.Password).Data;

        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = hashedPassword!,
            Name = request.Name
        };

        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(new CompanyModel { Name = request.Company.Name, Cnpj = request.Company.Cnpj });
        var adminRole = await roleRepository.GetAdminRoleAsync(cancellationToken);

        if (adminRole is null)
        {
            logger.LogCritical("Missing admin role. Please check database.");

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.InternalServerError, TextConstants.MissingAdminRole);
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

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Verifying user {UserID} existence in company {CompanyID}", userId, companyId);
        var response = await userRoleRepository.ExistsInCompanyAsync(userId, companyId, cancellationToken);

        return new ApiResponse<bool>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving user {UserID} information for token refresh", userId);
        var user = await userRepository.GetUserForRefreshTokenAsync(userId, cancellationToken);

        if (user is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
        }

        var response = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving user ID for email {Email}", email);

        var userId = await userRepository.GetUserIdFromEmailAsync(email, cancellationToken);

        if (userId is null)
        {
            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var response = new UserResponse
        {
            Id = userId.Value
        };

        return new ApiResponse<UserResponse>(response);
    }

    public async Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        logger.LogInformation("Initiating password change for user {UserID}", userId);
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("User not found {userID}", userId);

            return new ApiResponse<UserResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
        }

        var hashedPassword = securityService.HashPassword(password).Data;
        user.Password = hashedPassword!;
        await userRepository.SaveAsync(cancellationToken);

        var mapped = UserResponse.Convert(user);

        return new ApiResponse<UserResponse>(mapped);
    }
}
