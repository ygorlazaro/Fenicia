using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.ForgotPassword;

namespace Fenicia.Auth.Domains.ForgotPassword;

public static class ForgotPasswordMapper
{
    public static ForgotPasswordModel MapToForgotPasswordModel(ForgotPasswordRequest request, Guid userId, string code)
    {
        return new ForgotPasswordModel
        {
            UserId = userId,
            Code = code,
            ExpirationDate = DateTime.UtcNow.AddDays(1),
            IsActive = true,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        };
    }
}
