using MediatR;

namespace Fenicia.Auth.Domains.ForgotPassword.Commands;

public record AddForgotPasswordCommand(string Email, string? IpAddress = null, string? UserAgent = null) : IRequest;
