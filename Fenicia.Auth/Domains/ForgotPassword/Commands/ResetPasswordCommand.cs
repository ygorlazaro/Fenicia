using MediatR;

namespace Fenicia.Auth.Domains.ForgotPassword.Commands;

public sealed record ResetPasswordCommand(

    string Email,

    string Password,

    string Code) : IRequest;
