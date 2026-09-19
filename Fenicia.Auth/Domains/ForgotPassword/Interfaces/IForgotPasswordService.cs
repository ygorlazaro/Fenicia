using Fenicia.Common.DTOs.Auth.ForgotPassword;

namespace Fenicia.Auth.Domains.ForgotPassword.Interfaces;

public interface IForgotPasswordService
{
    Task AddAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    Task ResetAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}