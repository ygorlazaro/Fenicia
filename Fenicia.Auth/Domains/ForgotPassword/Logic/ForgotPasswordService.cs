namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using System.Net;

using AutoMapper;

using Common;

using Data;

using User.Logic;

/// <summary>
///     Service for handling password reset and forgot password functionality.
/// </summary>
public class ForgotPasswordService(IMapper mapper, ILogger<ForgotPasswordService> logger, IForgotPasswordRepository forgotPasswordRepository, IUserService userService) : IForgotPasswordService
{
    /// <summary>
    ///     Resets the password for a user using the provided reset code.
    /// </summary>
    /// <param name="request">The password reset request containing email, code, and new password.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>API response containing the result of the password reset operation.</returns>
    public async Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        logger.LogInformation(message: "Resetting password for user {email}", request.Email);
        var userId = await userService.GetUserIdFromEmailAsync(request.Email, cancellationToken);

        if (userId.Data is null)
        {
            logger.LogWarning(message: "User with email {email} not found", request.Email);
            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.InvalidUsernameOrPassword);
        }

        var currentCode = await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Data.Id, request.Code, cancellationToken);

        if (currentCode is null)
        {
            logger.LogWarning(message: "No code found for user {email}", request.Email);

            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ResetPasswordCodeNotFound);
        }

        await userService.ChangePasswordAsync(currentCode.UserId, request.Password, cancellationToken);
        await forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, cancellationToken);

        return new ApiResponse<ForgotPasswordResponse>(mapper.Map<ForgotPasswordResponse>(currentCode));
    }

    /// <summary>
    ///     Saves a forgot password request and generates a reset code.
    /// </summary>
    /// <param name="forgotPassword">The forgot password request containing the user's email.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>API response containing the generated forgot password record.</returns>
    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        logger.LogInformation(message: "Saving forgot password for user {email}", forgotPassword.Email);
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);

        if (userId.Data is null)
        {
            return new ApiResponse<ForgotPasswordResponse>(data: null, userId.Status, userId.Message);
        }

        var code = Guid.NewGuid().ToString().Replace(oldValue: "-", string.Empty)[..6];
        var request = new ForgotPasswordModel
                      {
                          Code = code,
                          IsActive = true,
                          UserId = userId.Data.Id
                      };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request, cancellationToken);

        return mapper.Map<ApiResponse<ForgotPasswordResponse>>(response);
    }
}
