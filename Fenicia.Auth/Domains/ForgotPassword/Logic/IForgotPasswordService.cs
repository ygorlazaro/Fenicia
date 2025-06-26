namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using Common;

using Data;

/// <summary>
///     Provides functionality for handling forgot password operations.
/// </summary>
public interface IForgotPasswordService
{
    /// <summary>
    ///     Resets the password for a user using the provided reset request.
    /// </summary>
    /// <param name="request">The password reset request containing email, code and new password.</param>
    /// <param name="cancellationToken">A token to observe cancellation requests.</param>
    /// <returns>An API response containing the forgot password operation result.</returns>
    Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken);

    /// <summary>
    ///     Initiates the forgot password process for a user.
    /// </summary>
    /// <param name="forgotPassword">The forgot password request containing the user's email.</param>
    /// <param name="cancellationToken">A token to observe cancellation requests.</param>
    /// <returns>An API response containing the forgot password operation result.</returns>
    Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken);
}
