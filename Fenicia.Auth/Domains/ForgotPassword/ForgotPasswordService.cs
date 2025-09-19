namespace Fenicia.Auth.Domains.ForgotPassword;

using System.Net;

using Common;
using Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using User;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly ILogger<ForgotPasswordService> logger;
    private readonly IForgotPasswordRepository forgotPasswordRepository;
    private readonly IUserService userService;

    public ForgotPasswordService(ILogger<ForgotPasswordService> logger, IForgotPasswordRepository forgotPasswordRepository, IUserService userService)
    {
        this.logger = logger;
        this.forgotPasswordRepository = forgotPasswordRepository;
        this.userService = userService;
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Resetting password for user {email}", request.Email);
        var userId = await this.userService.GetUserIdFromEmailAsync(request.Email, cancellationToken);

        if (userId.Data is null)
        {
            this.logger.LogWarning("User with email {email} not found", request.Email);
            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.InvalidUsernameOrPasswordMessage);
        }

        var currentCode = await this.forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Data.Id, request.Code, cancellationToken);

        if (currentCode is null)
        {
            this.logger.LogWarning("No code found for user {email}", request.Email);

            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ResetPasswordCodeNotFoundMessage);
        }

        await this.userService.ChangePasswordAsync(currentCode.UserId, request.Password, cancellationToken);
        await this.forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, cancellationToken);

        return new ApiResponse<ForgotPasswordResponse>(ForgotPasswordResponse.Convert(currentCode));
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Saving forgot password for user {email}", forgotPassword.Email);
        var userId = await this.userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);

        if (userId.Data is null)
        {
            return new ApiResponse<ForgotPasswordResponse>(data: null, userId.Status, userId.Message.Message);
        }

        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var request = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId.Data.Id
        };

        var response = await this.forgotPasswordRepository.SaveForgotPasswordAsync(request, cancellationToken);
        var mapped = ForgotPasswordResponse.Convert(response);

        return new ApiResponse<ForgotPasswordResponse>(mapped);
    }
}
