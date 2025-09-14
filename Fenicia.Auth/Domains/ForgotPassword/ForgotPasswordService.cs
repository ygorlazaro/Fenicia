namespace Fenicia.Auth.Domains.ForgotPassword;

using System.Net;

using Common;
using Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using Fenicia.Auth.Domains.User;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly ILogger<ForgotPasswordService> _logger;
    private readonly IForgotPasswordRepository _forgotPasswordRepository;
    private readonly IUserService _userService;

    public ForgotPasswordService(ILogger<ForgotPasswordService> logger, IForgotPasswordRepository forgotPasswordRepository, IUserService userService)
    {
        _logger = logger;
        _forgotPasswordRepository = forgotPasswordRepository;
        _userService = userService;
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> ResetPasswordAsync(ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting password for user {email}", request.Email);
        var userId = await _userService.GetUserIdFromEmailAsync(request.Email, cancellationToken);

        if (userId.Data is null)
        {
            _logger.LogWarning("User with email {email} not found", request.Email);
            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.InvalidUsernameOrPassword);
        }

        var currentCode = await _forgotPasswordRepository.GetFromUserIdAndCodeAsync(userId.Data.Id, request.Code, cancellationToken);

        if (currentCode is null)
        {
            _logger.LogWarning("No code found for user {email}", request.Email);

            return new ApiResponse<ForgotPasswordResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ResetPasswordCodeNotFound);
        }

        await _userService.ChangePasswordAsync(currentCode.UserId, request.Password, cancellationToken);
        await _forgotPasswordRepository.InvalidateCodeAsync(currentCode.Id, cancellationToken);

        return new ApiResponse<ForgotPasswordResponse>(ForgotPasswordResponse.Convert(currentCode));
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> SaveForgotPasswordAsync(ForgotPasswordRequest forgotPassword, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving forgot password for user {email}", forgotPassword.Email);
        var userId = await _userService.GetUserIdFromEmailAsync(forgotPassword.Email, cancellationToken);

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

        var response = await _forgotPasswordRepository.SaveForgotPasswordAsync(request, cancellationToken);
        var mapped = ForgotPasswordResponse.Convert(response);

        return new ApiResponse<ForgotPasswordResponse>(mapped);
    }
}
