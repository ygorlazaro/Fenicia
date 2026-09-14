using Fenicia.Common.DTOs.Auth.ForgotPassword;

namespace Fenicia.Auth.Domains.ForgotPassword.Interfaces;

public interface IForgotPasswordService
{
    Task AddAsync(AddForgotPasswordCommand command, CancellationToken cancellationToken = default);

    Task ResetAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default);
}