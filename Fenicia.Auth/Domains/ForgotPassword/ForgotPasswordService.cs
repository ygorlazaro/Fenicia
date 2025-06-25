using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.User;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordService(
    IMapper mapper,
    ILogger<ForgotPasswordService> logger,
    IForgotPasswordRepository forgotPasswordRepository,
    IUserService userService) : IForgotPasswordService
{
    public async Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(
        ForgotPasswordRequestReset request)
    {
        logger.LogInformation("Resetting password for user {email}", [request.Email]);
        var userId = await userService.GetUserIdFromEmailAsync(request.Email);

        if (userId.Data is null)
        {
            logger.LogWarning("User with email {email} not found", request.Email);
            return new ApiResponse<ForgotPasswordResponse>(null, HttpStatusCode.NotFound, TextConstants.InvalidUsernameOrPassword);
        }

        var currentCode =
            await forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Data.Id, request.Code);

        if (currentCode is null)
        {
            logger.LogWarning("No code found for user {email}", [request.Email]);

            return new ApiResponse<ForgotPasswordResponse>(null, HttpStatusCode.NotFound, TextConstants.ResetPasswordCodeNotFound);
        }

        await userService.ChangePasswordAsync(currentCode.UserId, request.Password);
        await forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id);

        return new ApiResponse<ForgotPasswordResponse>(mapper.Map<ForgotPasswordResponse>(currentCode));
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(
        ForgotPasswordRequest forgotPassword)
    {
        logger.LogInformation("Saving forgot password for user {email}", [forgotPassword.Email]);
        var userId = await userService.GetUserIdFromEmailAsync(forgotPassword.Email);

        if (userId.Data is null)
        {
            return new ApiResponse<ForgotPasswordResponse>(null, userId.Status, userId.Message);
        }

        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId.Data.Id
        };

        var response = await forgotPasswordRepository.SaveForgotPasswordAsync(request);

        return mapper.Map<ApiResponse<ForgotPasswordResponse>>(response);
    }
}
