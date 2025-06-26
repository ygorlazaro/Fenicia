namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using Data;

/// <summary>
///     Repository interface for managing forgot password functionality
/// </summary>
public interface IForgotPasswordRepository
{
    /// <summary>
    ///     Retrieves a forgot password record by user ID and verification code
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="code">The verification code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The forgot password model if found; otherwise, null</returns>
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken);

    /// <summary>
    ///     Invalidates a forgot password code
    /// </summary>
    /// <param name="id">The ID of the forgot password record to invalidate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Saves a new forgot password request
    /// </summary>
    /// <param name="forgotPasswordId">The forgot password model to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The saved forgot password model</returns>
    Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId, CancellationToken cancellationToken);
}
