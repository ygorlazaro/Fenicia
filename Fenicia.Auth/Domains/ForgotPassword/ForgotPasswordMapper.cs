using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.ForgotPassword;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
/// Mapper class for mapping between ForgotPasswordRequest and ForgotPasswordModel.
/// </summary>
public static class ForgotPasswordMapper
{
    /// <summary>
    /// Maps a ForgotPasswordRequest to a ForgotPasswordModel.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="code">The reset code.</param>
    /// <returns>The forgot password model.</returns>
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
