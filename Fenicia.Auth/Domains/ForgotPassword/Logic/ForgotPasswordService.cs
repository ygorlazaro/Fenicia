namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using System.Net;

using Common;

using Data;

using User.Logic;

public class ForgotPasswordService(ILogger<ForgotPasswordService> logger, IForgotPasswordRepository forgotPasswordRepository, IUserService userService) : IForgotPasswordService
{
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

        return new ApiResponse<ForgotPasswordResponse>(ForgotPasswordResponse.Convert(currentCode));
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        logger.LogInformation(message: "Saving forgot password for user {email}", forgotPassword.Email);
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);

        if (userId.Data is null)
        {
            return new ApiResponse<ForgotPasswordResponse>(data: null, userId.Status, userId.Message.Message);
        }

        var code = Guid.NewGuid().ToString().Replace(oldValue: "-", string.Empty)[..6];
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
