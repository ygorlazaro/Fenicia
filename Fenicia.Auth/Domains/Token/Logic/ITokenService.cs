using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Token.Logic;

/// <summary>
/// Service interface responsible for token generation and management operations.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user with their roles and permissions.
    /// </summary>
    /// <param name="user">The user information for token generation.</param>
    /// <param name="roles">Array of roles assigned to the user.</param>
    /// <param name="companyId">The company identifier associated with the user.</param>
    /// <param name="modules">List of modules the user has access to.</param>
    /// <returns>API response containing the generated token string if successful.</returns>
    ApiResponse<string> GenerateToken(
        [Required] UserResponse user,
        [Required] string[] roles,
        [Required] Guid companyId,
        [Required] List<ModuleType> modules
    );
}
