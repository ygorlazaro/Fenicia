using System.Net;

using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(ILogger<ForgotPasswordService> logger, IForgotPasswordRepository forgotPasswordRepository, IUserService userService) : IForgotPasswordService
{
    public async Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Resetting password for user {email}", request.Email);

        var userId = await userService.GetUserIdFromEmailAsync(request.Email, cancellationToken);

        if (userId.Data is null)
        {
            logger.LogWarning("User with email {email} not found", request.Email);

            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var currentCode = await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Data.Id, request.Code, cancellationToken);

        if (currentCode is null)
        {
            logger.LogWarning("No code found for user {email}", request.Email);

            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ResetPasswordCodeNotFoundMessage);
        }

        Task.WaitAll(
            userService.ChangePasswordAsync(currentCode.UserId, request.Password, cancellationToken),
            forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, cancellationToken));

        return new ApiResponse<ForgotPasswordResponse>(ForgotPasswordResponse.Convert(currentCode));
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saving forgot password for user {email}", forgotPassword.Email);

        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);

        if (userId.Data is null)
        {
            return new ApiResponse<ForgotPasswordResponse>(data: null, userId.Status, userId.Message?.Message);
        }

        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId.Data.Id
        };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request, cancellationToken);
        var mapped = ForgotPasswordResponse.Convert(response);

        return new ApiResponse<ForgotPasswordResponse>(mapped);
    }
}
