using Fenicia.Common.DTOs.Auth.ForgotPassword;

namespace Fenicia.Auth.Domains.ForgotPassword.Interfaces;

/// <summary>
/// Service interface for handling forgot password operations, including adding new requests and resetting passwords.
/// </summary>/
public interface IForgotPasswordService
{
    /// <summary>
    /// Adds a new forgot password request for a user. This method initiates the process of resetting a user's password by generating a unique code and sending it to the user's registered email address.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the password for a user based on the provided reset password request. This method validates the reset code and updates the user's password if the code is valid and has not expired.
    /// </summary>
    /// <param name="request">The reset password request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResetAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
