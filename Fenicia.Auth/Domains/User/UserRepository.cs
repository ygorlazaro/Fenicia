namespace Fenicia.Auth.Domains.User;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AuthContext authContext, ILogger<UserRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var query = from user in _authContext.Users join userRole in _authContext.UserRoles on user.Id equals userRole.UserId join company in _authContext.Companies on userRole.CompanyId equals company.Id where user.Email == email && company.Cnpj == cnpj select user;

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                _logger.LogInformation("User not found for email: {Email} and CNPJ: {Cnpj}", email, cnpj);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user for email: {Email} and CNPJ: {Cnpj}", email, cnpj);
            throw;
        }
    }

    public UserModel Add(UserModel userRequest)
    {
        _logger.LogInformation("Adding new user with email: {Email}", userRequest.Email);
        _authContext.Users.Add(userRequest);
        return userRequest;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully saved {Count} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _authContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
            _logger.LogInformation("User existence check for email {Email}: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence for email: {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation("User not found for refresh token with ID: {UserId}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user for refresh token with ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await _authContext.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
            if (userId == Guid.Empty)
            {
                _logger.LogInformation("No user ID found for email: {Email}", email);
            }

            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user ID for email: {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation("User not found with ID: {UserId}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
            throw;
        }
    }
}
