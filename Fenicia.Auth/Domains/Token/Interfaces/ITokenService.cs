using Fenicia.Common.DTOs.Auth.Token;

namespace Fenicia.Auth.Domains.Token.Interfaces;

/// <summary>
/// Service interface for managing authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the user based on the provided credentials.
    /// </summary>
    /// <param name="request">The token request containing email and password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the token response.</returns>
    Task<TokenResponse> GenerateAsync(TokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a JWT token string from a token response.
    /// </summary>
    /// <param name="user">The token response containing user information.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateString(TokenResponse user);
}